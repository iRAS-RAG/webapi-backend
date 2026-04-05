using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Interfaces.Telemetry;
using IRasRag.Application.Common.Models.Mqtt;
using IRasRag.Application.Common.Utils;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace IRasRag.Infrastructure.Services.Telemetry
{
    public class TelemetryDispatchService : ITelemetryDispatchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITelemetryCacheService _cache;
        private readonly ILogger<TelemetryDispatchService> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public TelemetryDispatchService(
            IUnitOfWork unitOfWork,
            ITelemetryCacheService cache,
            ILogger<TelemetryDispatchService> logger)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _logger = logger;
        }

        public async Task DispatchAsync(SensorTelemetry telemetry, string macFromTopic)
        {
            var timestamp = DateTime.UtcNow;
            var rawJson = JsonSerializer.Serialize(telemetry, JsonOptions);

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
                    telemetry, masterboard.Id, masterboard.FishTankId,
                    timestamp, rawJson, thresholds: null, batch: null);
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
                    telemetry, masterboard.Id, masterboard.FishTankId,
                    timestamp, rawJson, thresholds: null, batch: null);
                return;
            }

            // Step 3: Resolve sensors and thresholds
            var sensors = await _cache.GetSensorsByMasterboardAsync(masterboard.Id);
            var thresholds = await _cache.GetThresholdsAsync(stageConfig.SpeciesId, stageConfig.GrowthStageId);

            await PersistLogsAsync(
                telemetry, masterboard.Id, masterboard.FishTankId,
                timestamp, rawJson, thresholds, batch);
        }

        private async Task PersistLogsAsync(
            SensorTelemetry telemetry,
            Guid masterboardId,
            Guid tankId,
            DateTime timestamp,
            string rawJson,
            Dictionary<Guid, SpeciesThreshold>? thresholds,
            FarmingBatch? batch)
        {
            var sensors = await _cache.GetSensorsByMasterboardAsync(masterboardId);

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

                // Evaluate threshold if batch is active
                var isWarning = false;
                SpeciesThreshold? violatedThreshold = null;

                if (thresholds != null && thresholds.TryGetValue(sensor.SensorTypeId, out var threshold))
                {
                    isWarning = ThresholdEvaluator.IsViolation(reading.Val, threshold);
                    if (isWarning)
                        violatedThreshold = threshold;
                }
                else if (thresholds != null)
                {
                    _logger.LogInformation(
                        "No threshold configured for sensor type {SensorTypeId}, skipping alert evaluation",
                        sensor.SensorTypeId);
                }

                // Persist SensorLog
                var log = new SensorLog
                {
                    SensorId = sensor.Id,
                    Data = reading.Val,
                    IsWarning = isWarning,
                    DataJson = rawJson
                };

                await _unitOfWork.GetRepository<SensorLog>().AddAsync(log);

                // Persist Alert if violation found
                if (isWarning && violatedThreshold != null && batch != null)
                {
                    var alert = new Alert
                    {
                        SensorLogId = log.Id,
                        SpeciesThresholdId = violatedThreshold.Id,
                        FarmingBatchId = batch.Id,
                        FishTankId = tankId,
                        SensorTypeId = sensor.SensorTypeId,
                        Value = reading.Val,
                        RaisedAt = timestamp,
                        Status = AlertStatus.OPEN
                    };

                    await _unitOfWork.GetRepository<Alert>().AddAsync(alert);
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}