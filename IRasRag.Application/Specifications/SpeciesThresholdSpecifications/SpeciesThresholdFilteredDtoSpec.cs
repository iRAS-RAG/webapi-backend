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
                    SpeciesId = st.SpeciesId,
                    SpeciesName = st.Species.Name,
                    GrowthStageId = st.GrowthStageId,
                    GrowthStageName = st.GrowthStage.Name,
                    SensorTypeId = st.SensorTypeId,
                    SensorTypeName = st.SensorType.Name,
                    MinValue = st.MinValue,
                    MaxValue = st.MaxValue,
                });
        }
    }
}
