using IRasRag.Application.Common.Models.Mqtt;

namespace IRasRag.Application.Common.Interfaces.Telemetry
{
    public interface ITelemetryDispatchService
    {
        Task DispatchAsync(SensorTelemetry telemetry, string macFromTopic);
    }
}
