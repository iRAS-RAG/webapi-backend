using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications
{
    public class SpeciesThresholdByIdSpec : Specification<SpeciesThreshold, SpeciesThresholdDto>
    {
        public SpeciesThresholdByIdSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(st => st.Id == id)
                .Select(st => new SpeciesThresholdDto
                {
                    Id = st.Id,
                    SpeciesName = st.Species.Name,
                    GrowthStageName = st.GrowthStage.Name,
                    SensorTypeName = st.SensorType.Name,
                    MinValue = st.MinValue,
                    MaxValue = st.MaxValue,
                });
        }
    }

    public class SpeciesThresholdListSpec : Specification<SpeciesThreshold, SpeciesThresholdDto>
    {
        public SpeciesThresholdListSpec()
        {
            Query
                .AsNoTracking()
                .Select(st => new SpeciesThresholdDto
                {
                    Id = st.Id,
                    SpeciesName = st.Species.Name,
                    GrowthStageName = st.GrowthStage.Name,
                    SensorTypeName = st.SensorType.Name,
                    MinValue = st.MinValue,
                    MaxValue = st.MaxValue,
                });
        }
    }
}
