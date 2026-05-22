namespace IRasRag.Application.Common.Interfaces.Advisory
{
    public interface IThresholdSyncClient
    {
        Task<string> CreateAsync(
            string? userId,
            string species,
            string stage,
            string param,
            double min,
            double max,
            CancellationToken ct = default
        );

        Task UpdateAsync(string advisoryId, double min, double max, CancellationToken ct = default);

        Task DeleteAsync(string advisoryId, CancellationToken ct = default);
    }
}
