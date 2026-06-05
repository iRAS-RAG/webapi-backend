using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.SpeciesThresholdSpecifications
{
    public class SpeciesThresholdDtoBySpeciesIdSpec
        : Specification<SpeciesThreshold, SpeciesThresholdDto>
    {
        public SpeciesThresholdDtoBySpeciesIdSpec(Guid speciesId)
        {
            Query
                .AsNoTracking()
                .Where(st => st.Species.Id == speciesId)
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
