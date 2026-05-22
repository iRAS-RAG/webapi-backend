using IRasRag.Application.Common.Interfaces.Realtime;

namespace IRasRag.API.Hubs
{
    public class TelemetryPushWorker : BackgroundService
    {
        private readonly SignalRLiveDataNotifier _notifier;
        private readonly ILogger<TelemetryPushWorker> _logger;

        // Minimum interval between pushes for the same sensor — prevents flooding
        private static readonly TimeSpan ThrottleInterval = TimeSpan.FromMilliseconds(500);

        public TelemetryPushWorker(ILiveDataNotifier notifier, ILogger<TelemetryPushWorker> logger)
        {
            _notifier = (SignalRLiveDataNotifier)notifier;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var lastSent = new Dictionary<Guid, DateTime>();

            await foreach (var push in _notifier.Reader.ReadAllAsync(stoppingToken))
            {
                var now = DateTime.UtcNow;

                // Check if we've sent a push for this sensor recently
                if (
                    lastSent.TryGetValue(push.SensorId, out var last)
                    && (now - last) < ThrottleInterval
                )
                {
                    continue;
                }

                try
                {
                    await _notifier.PushTelemetryAsync(push);
                    lastSent[push.SensorId] = now;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        ex,
                        "SignalR push failed for sensor {SensorId}",
                        push.SensorId
                    );
                }
            }
        }
    }
}
