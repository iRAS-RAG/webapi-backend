using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.AlertSpecifications
{
    public class AlertContextSpec : Specification<Alert, AlertContext>
    {
        public AlertContextSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new AlertContext
                {
                    AlertId = a.Id,
                    TankName = a.FishTank.Name,
                    TankId = a.FishTankId,
                    FarmId = a.FishTank.FarmId,
                    SensorTypeName = a.SensorType.Name,
                    Unit = a.SensorType.UnitOfMeasure,
                    TriggerValue = a.TriggerValue,
                    MinThreshold = a.SpeciesThreshold.MinValue,
                    MaxThreshold = a.SpeciesThreshold.MaxValue,

                    SpeciesName =
                        a.FarmingBatch != null
                        && a.FarmingBatch.CurrentStageConfig != null
                        && a.FarmingBatch.CurrentStageConfig.Species != null
                            ? a.FarmingBatch.CurrentStageConfig.Species.Name
                            : "unknown",

                    StageName =
                        a.FarmingBatch != null
                        && a.FarmingBatch.CurrentStageConfig != null
                        && a.FarmingBatch.CurrentStageConfig.GrowthStage != null
                            ? a.FarmingBatch.CurrentStageConfig.GrowthStage.Name
                            : "unknown",
                });
        }
    }
}
