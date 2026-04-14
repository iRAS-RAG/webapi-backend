namespace IRasRag.Infrastructure.Services.Telemetry
{
    using IRasRag.Application.Common.Interfaces.Telemetry;
    using IRasRag.Application.Common.Models.Telemetry;
    using System.Collections.Concurrent;

    public class LatestTelemetryCacheService : ILatestTelemetryCacheService
    {
        private readonly ConcurrentDictionary<Guid, LiveReading> _cache = new();

        public void Set(Guid sensorId, double value, DateTime timestamp)
        {
            _cache.AddOrUpdate(
                sensorId,
                _ => new LiveReading(value, timestamp),  
                (_, existing) =>
                {
                    if (timestamp >= existing.Timestamp)
                        return new LiveReading (value, timestamp);

                    return existing;
                });
        }

        public LiveReading? Get(Guid sensorId)
        {
            if (_cache.TryGetValue(sensorId, out var reading))
                return reading;

            return null;
        }

        public IReadOnlyDictionary<Guid, LiveReading> GetMany(IEnumerable<Guid> sensorIds)
        {
            var result = new Dictionary<Guid, LiveReading>();

            foreach (var id in sensorIds)
            {
                if (_cache.TryGetValue(id, out var reading))
                {
                    result[id] = reading;
                }
            }

            return result;
        }
    }
}
