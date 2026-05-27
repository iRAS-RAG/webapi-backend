using System.Collections.Concurrent;
using IRasRag.Application.Common.Interfaces.Telemetry;
using IRasRag.Application.Common.Models.Realtime;

namespace IRasRag.Infrastructure.Services.Telemetry
{
    public class TelemetryWindowService : ITelemetryWindowService
    {
        private static readonly TimeSpan WindowDuration = TimeSpan.FromSeconds(10);

        private readonly ConcurrentDictionary<Guid, List<TelemetryPush>> _buffer = new();

        public void Append(TelemetryPush push)
        {
            var list = _buffer.GetOrAdd(push.TankId, _ => []);
            var cutoff = DateTime.UtcNow - WindowDuration;

            lock (list)
            {
                list.Add(push);
                list.RemoveAll(p => p.Timestamp < cutoff);
            }
        }

        public IReadOnlyList<TelemetryPush> GetWindow(Guid tankId)
        {
            if (!_buffer.TryGetValue(tankId, out var list))
                return [];

            var cutoff = DateTime.UtcNow - WindowDuration;

            lock (list)
                return list.Where(p => p.Timestamp >= cutoff).ToList();
        }
    }
}
