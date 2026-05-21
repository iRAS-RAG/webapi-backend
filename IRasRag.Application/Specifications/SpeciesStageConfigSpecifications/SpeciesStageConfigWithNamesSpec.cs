using Ardalis.Specification;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.SpeciesStageConfigSpecifications
{
    public sealed class SpeciesStageConfigWithNamesSpec : Specification<SpeciesStageConfig>
    {
        public SpeciesStageConfigWithNamesSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Include(c => c.Species)
                .Include(c => c.GrowthStage);
        }
    }
}
