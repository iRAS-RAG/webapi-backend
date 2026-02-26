using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.SpeciesThresholdSpecifications
{
    public class SpeciesThresholdFilteredDtoSpec
        : Specification<SpeciesThreshold, SpeciesThresholdDto>
    {
        public SpeciesThresholdFilteredDtoSpec(Expression<Func<SpeciesThreshold, bool>> predicate)
        {
            Query
                .AsNoTracking()
                .Where(predicate)
                .Select(st => new SpeciesThresholdDto
                {
                    Id = st.Id,
                    SpeciesName = st.Species.Name,
                    GrowthStageName = st.GrowthStage.Name,
                    Low = st.MinValue,
                    High = st.MaxValue,
                    Unit = st.SensorType.Name,
                });
        }
    }
}
