using IRasRag.Application.Common.Models.Realtime;

namespace IRasRag.Application.Common.Interfaces.Realtime
{
    public interface ILiveDataNotifier
    {
        Task PushTelemetryAsync(TelemetryPush push);
        Task PushAlertAsync(AlertPush push);
    }
}