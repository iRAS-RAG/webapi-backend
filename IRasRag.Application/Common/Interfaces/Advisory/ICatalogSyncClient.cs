namespace IRasRag.Application.Common.Interfaces.Advisory
{
    public interface ICatalogSyncClient
    {
        Task UpsertAsync(string code, string? labelVi, string? unit, CancellationToken ct = default);
        Task DeleteAsync(string code, CancellationToken ct = default);
    }
}
