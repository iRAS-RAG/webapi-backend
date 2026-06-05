using Ardalis.Specification;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.SpecificationHelpers
{
    public class SpecBySpeciesOrderedSpec : Specification<SpeciesStageConfig>
    {
        public SpecBySpeciesOrderedSpec(Guid speciesId)
        {
            Query.Where(s => s.SpeciesId == speciesId).OrderBy(s => s.Sequence);
        }
    }
}
