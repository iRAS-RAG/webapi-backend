using IRasRag.Application.Common.Models.Realtime;

namespace IRasRag.Application.Common.Interfaces.Telemetry
{
    public interface ITelemetryWindowService
    {
        void Append(TelemetryPush push);
        IReadOnlyList<TelemetryPush> GetWindow(Guid tankId);
    }
}
