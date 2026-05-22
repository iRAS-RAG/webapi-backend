namespace IRasRag.Application.Common.Interfaces.Advisory
{
    public interface ICatalogSyncJob
    {
        Task SyncUpsertAsync(string code, string? labelVi, string? unit);
        Task SyncDeleteAsync(string code);
        Task SyncAllAsync();
    }
}
