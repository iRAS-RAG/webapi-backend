using IRasRag.Application.Common.Interfaces.Advisory;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Interfaces.Realtime;
using IRasRag.Application.Common.Interfaces.Simulation;
using IRasRag.Application.Common.Interfaces.Telemetry;
using IRasRag.Application.Common.Models.Advisory;
using IRasRag.Application.Common.Models.Mqtt;
using IRasRag.Application.Common.Models.Realtime;
using IRasRag.Application.Common.Models.Telemetry;
using IRasRag.Application.Common.Utils;
using IRasRag.Domain.Entities;
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
        private readonly ITelemetryWindowService _telemetryWindow;
        private readonly ISimulationStateService _simulationState;

        public TelemetryDispatchService(
            IUnitOfWork unitOfWork,
            ITelemetryCacheService cache,
            ITelemetryLogBatchWriter logBatchWriter,
            ILatestTelemetryCacheService latestTelemetryCache,
            ILogger<TelemetryDispatchService> logger,
            IAlertStateEvaluator alertStateEvaluator,
            ILiveDataNotifier liveDataNotifier,
            IIoTIngestBatchWriter iotIngestBatchWriter,
            ITelemetryWindowService telemetryWindow,
            ISimulationStateService simulationState
        )
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _logBatchWriter = logBatchWriter;
            _latestTelemetryCache = latestTelemetryCache;
            _logger = logger;
            _alertStateEvaluator = alertStateEvaluator;
            _liveDataNotifier = liveDataNotifier;
            _iotIngestBatchWriter = iotIngestBatchWriter;
            _telemetryWindow = telemetryWindow;
            _simulationState = simulationState;
        }

        private static bool IsTemperatureSensor(Sensor? sensor)
        {
            var name = sensor?.SensorType?.Name ?? "";
            return name.Contains("nhiệt", StringComparison.OrdinalIgnoreCase)
                || name.Contains("temp", StringComparison.OrdinalIgnoreCase)
                || name.Contains("nhiệt", StringComparison.OrdinalIgnoreCase);
        }

        public async Task DispatchAsync(SensorTelemetry telemetry, string macFromTopic)
        {
            var timestamp = DateTime.UtcNow;
            var isSimulating = _simulationState.IsSimulating(macFromTopic);

            if (isSimulating)
            {
                _logger.LogInformation(
                    "MAC {MacAddress} is in simulation mode — temperature readings will be replaced with random 50-60°C values",
                    macFromTopic
                );
            }

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
                    masterboard.FishTankId
                );

                await PersistLogsAsync(
                    telemetry,
                    masterboard.Id,
                    masterboard.FishTank.FarmId,
                    masterboard.FishTankId,
                    timestamp,
                    thresholds: null,
                    batch: null,
                    species: null,
                    stage: null,
                    tankName: masterboard.FishTank.Name,
                    isSimulating: isSimulating
                );
                return;
            }

            // Step 2b: Resolve SpeciesStageConfig from batch
            var stageConfig = await _cache.GetStageConfigAsync(batch.CurrentStageConfigId);
            if (stageConfig == null)
            {
                _logger.LogWarning(
                    "No SpeciesStageConfig found for batch {BatchId}, persisting logs without alert evaluation",
                    batch.Id
                );

                await PersistLogsAsync(
                    telemetry,
                    masterboard.Id,
                    masterboard.FishTank.FarmId,
                    masterboard.FishTankId,
                    timestamp,
                    thresholds: null,
                    batch: null,
                    species: null,
                    stage: null,
                    tankName: masterboard.FishTank.Name,
                    isSimulating: isSimulating
                );
                return;
            }

            // Step 3: Resolve sensors and thresholds
            var thresholds = await _cache.GetThresholdsAsync(
                stageConfig.SpeciesId,
                stageConfig.GrowthStageId
            );

            await PersistLogsAsync(
                telemetry,
                masterboard.Id,
                masterboard.FishTank.FarmId,
                masterboard.FishTankId,
                timestamp,
                thresholds,
                batch,
                species: stageConfig.Species.Name,
                stage: stageConfig.GrowthStage.Name,
                tankName: masterboard.FishTank.Name,
                isSimulating: isSimulating
            );
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
            string? stage,
            string tankName,
            bool isSimulating = false
        )
        {
            var sensors = await _cache.GetSensorsByMasterboardAsync(masterboardId);
            var bufferedLogs = new List<TelemetryLogWriteModel>();
            var metrics = new List<IoTMetric>();
            var rng = Random.Shared;

            foreach (var reading in telemetry.Readings)
            {
                // Resolve sensor by pin
                if (!sensors.TryGetValue(reading.Pin, out var sensor))
                {
                    _logger.LogWarning(
                        "No sensor found for pin {Pin} on masterboard {MasterboardId}, skipping",
                        reading.Pin,
                        masterboardId
                    );
                    continue;
                }

                // Override reading value if in simulation mode and this is a temperature sensor
                var finalValue = reading.Val;
                if (isSimulating && IsTemperatureSensor(sensor))
                {
                    finalValue = Math.Round(50.0 + rng.NextDouble() * 10.0, 2); // Random 50.00 - 60.00
                }

                // Update latest cache for this sensor
                _latestTelemetryCache.Set(sensor.Id, finalValue, timestamp);

                // Enqueue live reading — decoupled from ingestion path via Channel<T>
                var push = new TelemetryPush(
                    sensor.Id,
                    tankId,
                    finalValue,
                    timestamp,
                    sensor.SensorType?.Name
                );
                _liveDataNotifier.EnqueueTelemetry(push);
                _telemetryWindow.Append(push);

                // Evaluate thresholds and prepare log entry
                var isWarning = false;

                if (
                    thresholds != null
                    && thresholds.TryGetValue(sensor.SensorTypeId, out var threshold)
                )
                {
                    isWarning = ThresholdEvaluator.IsViolation(finalValue, threshold);

                    if (batch != null)
                    {
                        await _alertStateEvaluator.EvaluateAsync(
                            tankId,
                            sensor.Id,
                            sensor.SensorTypeId,
                            batch.Id,
                            threshold,
                            finalValue,
                            tankName,
                            sensor.SensorType?.Name
                        );
                    }
                }
                else if (thresholds == null)
                {
                    _logger.LogInformation(
                        "No threshold configured for sensor type {SensorTypeId}, skipping alert evaluation",
                        sensor.SensorTypeId
                    );
                }

                bufferedLogs.Add(
                    new TelemetryLogWriteModel
                    {
                        SensorId = sensor.Id,
                        Data = finalValue,
                        IsWarning = isWarning,
                        CreatedAt = timestamp,
                    }
                );

                if (sensor.SensorType?.Code != null)
                    metrics.Add(
                        new IoTMetric
                        {
                            Code = sensor.SensorType.Code,
                            Value = finalValue,
                            Unit = sensor.SensorType.UnitOfMeasure,
                        }
                    );
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
            List<IoTMetric> metrics
        )
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
                    Metrics = metrics.Count > 0 ? metrics : null,
                };

                _iotIngestBatchWriter.Enqueue(payload);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Advisory IoT enqueue failed for farm {FarmId}/tank {TankId} — continuing",
                    farmId,
                    tankId
                );
            }
        }
    }
}
