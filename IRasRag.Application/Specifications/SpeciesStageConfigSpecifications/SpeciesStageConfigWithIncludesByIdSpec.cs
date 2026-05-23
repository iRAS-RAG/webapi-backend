using Ardalis.Specification;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.SpeciesStageConfigSpecifications
{
    public class SpeciesStageConfigWithIncludesByIdSpec : Specification<SpeciesStageConfig>
    {
        public SpeciesStageConfigWithIncludesByIdSpec(Guid id)
        {
            Query
                .Where(ssc => ssc.Id == id)
                .Include(ssc => ssc.GrowthStage)
                .Include(ssc => ssc.Species)
                .Include(ssc => ssc.FeedTypes)
                .AsNoTracking();
        }
    }
}
