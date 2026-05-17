namespace IRasRag.Application.Common.Interfaces.Advisory
{
    public interface IThresholdSyncJob
    {
        Task SyncCreateAsync(Guid thresholdId, string? userId);
        Task SyncUpdateAsync(string advisoryId, double min, double max);
        Task SyncDeleteAsync(string advisoryId);
    }
}
