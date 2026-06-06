using IRasRag.Application.Common.Interfaces.Advisory;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Infrastructure.Services.Advisory.Jobs
{
    public class CatalogSyncJob : ICatalogSyncJob
    {
        private readonly ICatalogSyncClient _syncClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CatalogSyncJob> _logger;

        public CatalogSyncJob(
            ICatalogSyncClient syncClient,
            IUnitOfWork unitOfWork,
            ILogger<CatalogSyncJob> logger
        )
        {
            _syncClient = syncClient;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task SyncUpsertAsync(string code, string? labelVi, string? unit)
        {
            await _syncClient.UpsertAsync(code, labelVi, unit);

            _logger.LogInformation("Advisory catalog upsert succeeded for code '{Code}'", code);
        }

        public async Task SyncDeleteAsync(string code)
        {
            await _syncClient.DeleteAsync(code);

            _logger.LogInformation("Advisory catalog delete succeeded for code '{Code}'", code);
        }

        public async Task SyncAllAsync()
        {
            var repo = _unitOfWork.GetRepository<SensorType>();
            var all = await repo.FindAllAsync(st => st.Code != null);

            _logger.LogInformation(
                "Advisory catalog bulk sync starting for {Count} sensor type(s)",
                all.Count
            );

            foreach (var st in all)
                await _syncClient.UpsertAsync(st.Code!, st.Name, st.UnitOfMeasure);

            _logger.LogInformation("Advisory catalog bulk sync completed");
        }
    }
}
