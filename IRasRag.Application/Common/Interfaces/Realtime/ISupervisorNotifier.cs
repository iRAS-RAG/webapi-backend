namespace IRasRag.Application.Common.Interfaces.Realtime
{
    public interface ISupervisorNotifier
    {
        Task NotifyFeedingLogAsync(Guid farmId, object payload);
        Task NotifyMortalityLogAsync(Guid farmId, object payload);
    }
}
