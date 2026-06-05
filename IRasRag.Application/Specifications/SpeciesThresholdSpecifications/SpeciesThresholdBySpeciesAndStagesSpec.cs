using Ardalis.Specification;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.SpeciesThresholdSpecifications
{
    /// <summary>
    /// Non-projected spec — returns full <see cref="SpeciesThreshold"/> entities
    /// (including <see cref="SpeciesThreshold.SensorType"/>) filtered by species
    /// and a set of growth-stage IDs.
    /// </summary>
    public class SpeciesThresholdBySpeciesAndStagesSpec : Specification<SpeciesThreshold>
    {
        public SpeciesThresholdBySpeciesAndStagesSpec(
            Guid speciesId,
            IEnumerable<Guid> growthStageIds
        )
        {
            var stageIds = growthStageIds.ToList();

            Query
                .AsNoTracking()
                .Include(st => st.SensorType)
                .Where(st => st.SpeciesId == speciesId && stageIds.Contains(st.GrowthStageId));
        }
    }
}
