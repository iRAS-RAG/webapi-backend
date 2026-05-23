using Ardalis.Specification;
using IRasRag.Application.DTOs;
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
                .AsNoTracking();
        }
    }
}
