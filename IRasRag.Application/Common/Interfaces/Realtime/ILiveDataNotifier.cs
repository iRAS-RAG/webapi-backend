using IRasRag.Application.Common.Models.Realtime;

namespace IRasRag.Application.Common.Interfaces.Realtime
{
    public interface ILiveDataNotifier
    {
        void EnqueueTelemetry(TelemetryPush push);
        Task PushTelemetryAsync(TelemetryPush push);
    }
}