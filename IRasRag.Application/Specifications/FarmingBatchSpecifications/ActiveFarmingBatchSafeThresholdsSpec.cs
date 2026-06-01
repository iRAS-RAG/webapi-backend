using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.FarmingBatchSpecifications
{
    public class ActiveFarmingBatchSafeThresholdsSpec
        : Specification<SpeciesThreshold, ActiveFarmingBatchSafeThresholdDto>
    {
        public ActiveFarmingBatchSafeThresholdsSpec(Guid speciesId, Guid growthStageId)
        {
            Query
                .AsNoTracking()
                .Where(st => st.SpeciesId == speciesId && st.GrowthStageId == growthStageId)
                .Include(st => st.SensorType)
                .Select(st => new ActiveFarmingBatchSafeThresholdDto
                {
                    SensorTypeId = st.SensorTypeId,
                    SensorTypeName = st.SensorType.Name,
                    UnitOfMeasure = st.SensorType.UnitOfMeasure,
                    MinValue = st.MinValue,
                    MaxValue = st.MaxValue,
                });
        }
    }
}
