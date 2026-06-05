using IRasRag.Application.Common.Models.Telemetry;

namespace IRasRag.Application.Common.Interfaces.Telemetry
{
    public interface ILatestTelemetryCacheService
    {
        void Set(Guid sensorId, double value, DateTime timestamp);

        LiveReading? Get(Guid sensorId);

        IReadOnlyDictionary<Guid, LiveReading> GetMany(IEnumerable<Guid> sensorIds);
    }
}
