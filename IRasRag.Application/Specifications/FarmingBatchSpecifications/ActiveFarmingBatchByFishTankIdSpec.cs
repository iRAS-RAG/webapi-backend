using Ardalis.Specification;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;

namespace IRasRag.Application.Specifications.FarmingBatchSpecifications
{
    public class ActiveFarmingBatchByFishTankIdSpec : Specification<FarmingBatch>
    {
        public ActiveFarmingBatchByFishTankIdSpec(Guid fishTankId)
        {
            Query
                .Where(fb => fb.Status == FarmingBatchStatus.ACTIVE && fb.FishTankId == fishTankId)
                .Include(fb => fb.FishTank)
                .Include(fb => fb.CurrentStageConfig)
                    .ThenInclude(ssc => ssc.Species)
                .Include(fb => fb.CurrentStageConfig)
                    .ThenInclude(ssc => ssc.GrowthStage)
                .AsNoTracking();
        }
    }
}
