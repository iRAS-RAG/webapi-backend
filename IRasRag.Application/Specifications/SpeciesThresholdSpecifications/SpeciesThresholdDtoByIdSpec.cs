using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.SpeciesThresholdSpecifications
{
    public class SpeciesThresholdDtoByIdSpec : Specification<SpeciesThreshold, SpeciesThresholdDto>
    {
        public SpeciesThresholdDtoByIdSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(st => st.Id == id)
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
                    UnitOfMeasure = st.SensorType.UnitOfMeasure,
                });
        }
    }
}
