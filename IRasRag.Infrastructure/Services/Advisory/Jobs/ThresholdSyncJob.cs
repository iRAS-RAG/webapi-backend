using IRasRag.Application.Common.Interfaces.Advisory;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Specifications.SpeciesThresholdSpecifications;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Infrastructure.Services.Advisory.Jobs
{
    public class ThresholdSyncJob : IThresholdSyncJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IThresholdSyncClient _syncClient;
        private readonly ILogger<ThresholdSyncJob> _logger;

        public ThresholdSyncJob(
            IUnitOfWork unitOfWork,
            IThresholdSyncClient syncClient,
            ILogger<ThresholdSyncJob> logger
        )
        {
            _unitOfWork = unitOfWork;
            _syncClient = syncClient;
            _logger = logger;
        }

        public async Task SyncCreateAsync(Guid thresholdId, string? userId)
        {
            var threshold = await _unitOfWork
                .GetRepository<SpeciesThreshold>()
                .GetByIdAsync(thresholdId);

            if (threshold == null)
            {
                _logger.LogWarning(
                    "Advisory sync create skipped: threshold {ThresholdId} no longer exists",
                    thresholdId
                );
                return;
            }

            var dto = await _unitOfWork
                .GetRepository<SpeciesThreshold>()
                .FirstOrDefaultAsync(new SpeciesThresholdDtoByIdSpec(thresholdId));

            if (dto == null)
                throw new InvalidOperationException(
                    $"Advisory sync create failed: DTO not found for threshold {thresholdId}"
                );

            var advisoryId = await _syncClient.CreateAsync(
                userId,
                dto.SpeciesName,
                dto.GrowthStageName,
                dto.SensorTypeName,
                dto.MinValue,
                dto.MaxValue
            );

            threshold.AdvisoryThresholdId = advisoryId;
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Advisory sync create succeeded: threshold {ThresholdId} â†’ advisory {AdvisoryId}",
                thresholdId,
                advisoryId
            );
        }

        public async Task SyncUpdateAsync(string advisoryId, double min, double max)
        {
            await _syncClient.UpdateAsync(advisoryId, min, max);

            _logger.LogInformation(
                "Advisory sync update succeeded for advisory threshold {AdvisoryId}",
                advisoryId
            );
        }

        public async Task SyncDeleteAsync(string advisoryId)
        {
            await _syncClient.DeleteAsync(advisoryId);

            _logger.LogInformation(
                "Advisory sync delete succeeded for advisory threshold {AdvisoryId}",
                advisoryId
            );
        }
    }
}

