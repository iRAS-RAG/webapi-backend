using IRasRag.Application.Common.Interfaces.Advisory;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Interfaces.Realtime;
using IRasRag.Application.Common.Interfaces.Telemetry;
using IRasRag.Application.Common.Models.Advisory;
using IRasRag.Application.Common.Models.Mqtt;
using IRasRag.Application.Common.Models.Realtime;
using IRasRag.Application.Common.Models.Telemetry;
using IRasRag.Application.Common.Utils;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IRasRag.Infrastructure.Services.Telemetry
{
    public class TelemetryDispatchService : ITelemetryDispatchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITelemetryCacheService _cache;
        private readonly ITelemetryLogBatchWriter _logBatchWriter;
        private readonly ILatestTelemetryCacheService _latestTelemetryCache;
        private readonly ILogger<TelemetryDispatchService> _logger;
        private readonly IAlertStateEvaluator _alertStateEvaluator;
        private readonly ILiveDataNotifier _liveDataNotifier;
        private readonly IIoTIngestBatchWriter _iotIngestBatchWriter;

        public TelemetryDispatchService(
            IUnitOfWork unitOfWork,
            ITelemetryCacheService cache,
            ITelemetryLogBatchWriter logBatchWriter,
            ILatestTelemetryCacheService latestTelemetryCache,
            ILogger<TelemetryDispatchService> logger,
            IAlertStateEvaluator alertStateEvaluator,
            ILiveDataNotifier liveDataNotifier,
            IIoTIngestBatchWriter iotIngestBatchWriter)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _logBatchWriter = logBatchWriter;
            _latestTelemetryCache = latestTelemetryCache;
            _logger = logger;
            _alertStateEvaluator = alertStateEvaluator;
            _liveDataNotifier = liveDataNotifier;
            _iotIngestBatchWriter = iotIngestBatchWriter;
        }

        public async Task DispatchAsync(SensorTelemetry telemetry, string macFromTopic)
        {
            var timestamp = DateTime.UtcNow;

            // Step 1: Resolve masterboard
            var masterboard = await _cache.GetMasterboardByMacAsync(macFromTopic);
            if (masterboard == null)
            {
                _logger.LogWarning("Unknown MAC {Mac}, skipping ingestion", macFromTopic);
                return;
            }

            // Step 2: Resolve active farming batch
            var batch = await _cache.GetActiveBatchAsync(masterboard.FishTankId);
            if (batch == null)
            {
                _logger.LogInformation(
                    "No active batch for tank {TankId}, persisting logs without alert evaluation",
                    masterboard.FishTankId);

                await PersistLogsAsync(
                    telemetry, masterboard.Id, masterboard.FishTank.FarmId, masterboard.FishTankId,
                    timestamp, thresholds: null, batch: null, species: null, stage: null);
                return;
            }

            // Step 2b: Resolve SpeciesStageConfig from batch
            var stageConfig = await _cache.GetStageConfigAsync(batch.CurrentStageConfigId);
            if (stageConfig == null)
            {
                _logger.LogWarning(
                    "No SpeciesStageConfig found for batch {BatchId}, persisting logs without alert evaluation",
                    batch.Id);

                await PersistLogsAsync(
                    telemetry, masterboard.Id, masterboard.FishTank.FarmId, masterboard.FishTankId,
                    timestamp, thresholds: null, batch: null, species: null, stage: null);
                return;
            }

            // Step 3: Resolve sensors and thresholds
            var thresholds = await _cache.GetThresholdsAsync(stageConfig.SpeciesId, stageConfig.GrowthStageId);

            await PersistLogsAsync(
                telemetry, masterboard.Id, masterboard.FishTank.FarmId, masterboard.FishTankId,
                timestamp, thresholds, batch,
                species: stageConfig.Species.Name,
                stage: stageConfig.GrowthStage.Name);
        }

        private async Task PersistLogsAsync(
            SensorTelemetry telemetry,
            Guid masterboardId,
            Guid farmId,
            Guid tankId,
            DateTime timestamp,
            Dictionary<Guid, SpeciesThreshold>? thresholds,
            FarmingBatch? batch,
            string? species,
            string? stage)
        {
            var sensors = await _cache.GetSensorsByMasterboardAsync(masterboardId);
            var bufferedLogs = new List<TelemetryLogWriteModel>();
            var metrics = new List<IoTMetric>();

            foreach (var reading in telemetry.Readings)
            {
                // Resolve sensor by pin
                if (!sensors.TryGetValue(reading.Pin, out var sensor))
                {
                    _logger.LogWarning(
                        "No sensor found for pin {Pin} on masterboard {MasterboardId}, skipping",
                        reading.Pin, masterboardId);
                    continue;
                }

                // Update latest cache for this sensor
                _latestTelemetryCache.Set(sensor.Id, reading.Val, timestamp);

                // Enqueue live reading — decoupled from ingestion path via Channel<T>
                _liveDataNotifier.EnqueueTelemetry(
                    new TelemetryPush(sensor.Id, tankId, reading.Val, timestamp));

                // Evaluate thresholds and prepare log entry
                var isWarning = false;

                if (thresholds != null && thresholds.TryGetValue(sensor.SensorTypeId, out var threshold))
                {
                    isWarning = ThresholdEvaluator.IsViolation(reading.Val, threshold);

                    if (batch != null)
                    {
                        await _alertStateEvaluator.EvaluateAsync(
                            tankId, sensor.Id, sensor.SensorTypeId, batch.Id, threshold, reading.Val,
                            sensor.Name, batch.Name);
                    }
                }
                else if (thresholds == null)
                {
                    _logger.LogInformation(
                        "No threshold configured for sensor type {SensorTypeId}, skipping alert evaluation",
                        sensor.SensorTypeId);
                }

                bufferedLogs.Add(
                    new TelemetryLogWriteModel
                    {
                        SensorId = sensor.Id,
                        Data = reading.Val,
                        IsWarning = isWarning,
                        CreatedAt = timestamp
                    });

                if (sensor.SensorType?.Code != null)
                    metrics.Add(new IoTMetric
                    {
                        Code = sensor.SensorType.Code,
                        Value = reading.Val,
                        Unit = sensor.SensorType.UnitOfMeasure
                    });
            }

            if (bufferedLogs.Count > 0)
                await _logBatchWriter.EnqueueBatchAsync(bufferedLogs);

            TryIngestAdvisoryAsync(farmId, tankId, timestamp, species, stage, metrics);
        }

        private void TryIngestAdvisoryAsync(
            Guid farmId,
            Guid tankId,
            DateTime timestamp,
            string? species,
            string? stage,
            List<IoTMetric> metrics)
        {
            try
            {
                var payload = new IoTIngestPayload
                {
                    FarmId = farmId.ToString(),
                    TankId = tankId.ToString(),
                    Ts = timestamp.ToString("o"),
                    Species = species,
                    Stage = stage,
                    Metrics = metrics.Count > 0 ? metrics : null
                };

                _iotIngestBatchWriter.Enqueue(payload);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Advisory IoT enqueue failed for farm {FarmId}/tank {TankId} — continuing",
                    farmId, tankId);
            }
        }
    }
}