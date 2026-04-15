using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Interfaces.Realtime;
using IRasRag.Application.Common.Interfaces.Telemetry;
using IRasRag.Application.Common.Models.Telemetry;
using IRasRag.Application.Common.Settings;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IRasRag.Application.Common.Utils
{
    public class AlertStateEvaluator : IAlertStateEvaluator
    {
        private readonly IAlertStateCacheService _alertStateCache;
        private readonly AlertSettings _settings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AlertStateEvaluator> _logger;
        private readonly ILiveDataNotifier _liveDataNotifier;

        public AlertStateEvaluator(
            IAlertStateCacheService alertStateCache,
            IOptions<AlertSettings> settings,
            IUnitOfWork unitOfWork,
            ILogger<AlertStateEvaluator> logger,
            ILiveDataNotifier liveDataNotifier)
        {
            _alertStateCache = alertStateCache ?? throw new ArgumentNullException(nameof(alertStateCache));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _liveDataNotifier = liveDataNotifier ?? throw new ArgumentNullException(nameof(liveDataNotifier));

            ValidateSettings(_settings);
        }

        public async Task EvaluateAsync(
            Guid tankId,
            Guid sensorId,
            Guid sensorTypeId,
            Guid? batchId,
            SpeciesThreshold threshold,
            double value)
        {
            ArgumentNullException.ThrowIfNull(threshold);

            try
            {
                var state = await _alertStateCache.Get(tankId, sensorTypeId, batchId)
                            ?? new AlertState();

                var isBreach = value < threshold.MinValue || value > threshold.MaxValue;
                var isInRecoveryZone = IsInRecoveryZone(value, threshold);

                if (state.ActiveAlertId.HasValue)
                {
                    await HandleActiveAlertAsync(tankId, sensorTypeId, batchId, threshold, value, state, isBreach, isInRecoveryZone);
                    return;
                }

                await HandleNoActiveAlertAsync(tankId, sensorId, sensorTypeId, batchId, threshold, value, state, isBreach);
            }
            catch (Exception ex)
            {
                // Log and swallow � one bad reading should never stop ingestion
                _logger.LogError(ex,
                    "Alert evaluation failed for tank {TankId}, sensorType {SensorTypeId}. Value: {Value}",
                    tankId, sensorTypeId, value);
            }
        }

        private async Task HandleActiveAlertAsync(
            Guid tankId,
            Guid sensorTypeId,
            Guid? batchId,
            SpeciesThreshold threshold,
            double value,
            AlertState state,
            bool isBreach,
            bool isInRecoveryZone)
        {
            if (isBreach)
            {
                _alertStateCache.Set(tankId, sensorTypeId, batchId, new AlertState
                {
                    ActiveAlertId = state.ActiveAlertId,
                    BreachCount = state.BreachCount + 1,
                    ResolveCount = 0,
                    LastValue = value
                });
                return;
            }

            if (!isInRecoveryZone)
            {
                // Still near threshold edge; keep alert open and require consecutive stable samples.
                _alertStateCache.Set(tankId, sensorTypeId, batchId, new AlertState
                {
                    ActiveAlertId = state.ActiveAlertId,
                    BreachCount = state.BreachCount,
                    ResolveCount = 0,
                    LastValue = value
                });
                return;
            }

            var nextResolveCount = state.ResolveCount + 1;
            if (nextResolveCount < _settings.ResolveCount)
            {
                _alertStateCache.Set(tankId, sensorTypeId, batchId, new AlertState
                {
                    ActiveAlertId = state.ActiveAlertId,
                    BreachCount = state.BreachCount,
                    ResolveCount = nextResolveCount,
                    LastValue = value
                });
                return;
            }

            var alert = await _unitOfWork.GetRepository<Alert>().GetByIdAsync(state.ActiveAlertId!.Value);
            if (alert != null && (alert.Status == AlertStatus.OPEN || alert.Status == AlertStatus.ACKNOWLEDGED))
            {
                alert.ResolvedAt = DateTime.UtcNow;
                alert.Status = AlertStatus.RESOLVED;
                _unitOfWork.GetRepository<Alert>().Update(alert);
                await _unitOfWork.SaveChangesAsync();
            }

            _alertStateCache.Invalidate(tankId, sensorTypeId, batchId);
        }

        private async Task HandleNoActiveAlertAsync(
            Guid tankId,
            Guid sensorId,
            Guid sensorTypeId,
            Guid? batchId,
            SpeciesThreshold threshold,
            double value,
            AlertState state,
            bool isBreach)
        {
            if (!isBreach)
            {
                _alertStateCache.Set(tankId, sensorTypeId, batchId, new AlertState
                {
                    ActiveAlertId = null,
                    BreachCount = 0,
                    ResolveCount = 0,
                    LastValue = value
                });
                return;
            }

            var nextBreachCount = state.BreachCount + 1;
            if (nextBreachCount < _settings.BreachConfirmationCount)
            {
                _alertStateCache.Set(tankId, sensorTypeId, batchId, new AlertState
                {
                    ActiveAlertId = null,
                    BreachCount = nextBreachCount,
                    ResolveCount = 0,
                    LastValue = value
                });
                return;
            }

            var newAlert = new Alert
            {
                Id = Guid.NewGuid(),
                FishTankId = tankId,
                SensorId = sensorId,
                SensorTypeId = sensorTypeId,
                FarmingBatchId = batchId,
                SpeciesThresholdId = threshold.Id,
                TriggerValue = value,
                RaisedAt = DateTime.UtcNow,
                Status = AlertStatus.OPEN
            };

            await _unitOfWork.GetRepository<Alert>().AddAsync(newAlert);
            await _unitOfWork.SaveChangesAsync();

            _alertStateCache.Set(tankId, sensorTypeId, batchId, new AlertState
            {
                ActiveAlertId = newAlert.Id,
                BreachCount = nextBreachCount,
                ResolveCount = 0,
                LastValue = value
            });

        }

        private bool IsInRecoveryZone(double value, SpeciesThreshold threshold)
        {
            var span = threshold.MaxValue - threshold.MinValue;
            var hysteresisAmount = span * _settings.HysteresisPercentage;

            var lowerBound = threshold.MinValue + hysteresisAmount;
            var upperBound = threshold.MaxValue - hysteresisAmount;

            if (lowerBound > upperBound)
            {
                var midpoint = (threshold.MinValue + threshold.MaxValue) / 2.0;
                lowerBound = midpoint;
                upperBound = midpoint;
            }

            return value >= lowerBound && value <= upperBound;
        }

        private static void ValidateSettings(AlertSettings settings)
        {
            if (settings.BreachConfirmationCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(settings.BreachConfirmationCount), "MaxBreachCount must be >= 1.");
            }

            if (settings.ResolveCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(settings.ResolveCount), "ResolveCount must be >= 1.");
            }

            if (settings.HysteresisPercentage < 0 || settings.HysteresisPercentage >= 0.5)
            {
                throw new ArgumentOutOfRangeException(nameof(settings.HysteresisPercentage), "HysteresisPercentage must be in range [0, 0.5).");
            }
        }
    }
}