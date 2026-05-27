using Ardalis.Specification;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.FarmingBatchSpecifications
{
    public class FarmingBatchWithStagesByIdSpec : Specification<FarmingBatch>
    {
        public FarmingBatchWithStagesByIdSpec(Guid id)
        {
            Query
                .Where(fb => fb.Id == id)
                .Include(fb => fb.BatchStages)
                    .ThenInclude(bs => bs.SpeciesStageConfig)
                        .ThenInclude(ssc => ssc.GrowthStage)
                .Include(fb => fb.BatchStages)
                    .ThenInclude(bs => bs.SpeciesStageConfig)
                        .ThenInclude(ssc => ssc.Species)
                .Include(fb => fb.BatchStages)
                    .ThenInclude(bs => bs.SpeciesStageConfig)
                        .ThenInclude(ssc => ssc.FeedTypes)
                .AsNoTracking();
        }
    }
}
