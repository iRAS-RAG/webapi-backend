using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.AlertSpecifications
{
    public class AlertDtoByIdSpec : Specification<Alert, AlertDto>
    {
        public AlertDtoByIdSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new AlertDto
                {
                    Id = a.Id,
                    SensorId = a.SensorId,
                    SpeciesThresholdId = a.SpeciesThresholdId,
                    FarmingBatchId = a.FarmingBatchId,
                    FarmingBatchName = a.FarmingBatch != null ? a.FarmingBatch.Name : null,
                    FishTankId = a.FishTankId,
                    FishTankName = a.FishTank.Name,
                    SensorTypeName = a.SensorType.Name,
                    TriggerValue = a.TriggerValue,
                    RaisedAt = a.RaisedAt,
                    ResolvedAt = a.ResolvedAt,
                    Status = a.Status,
                    UnitOfMeasure = a.SensorType.UnitOfMeasure,
                    MinThreshold = a.SpeciesThreshold.MinValue,
                    MaxThreshold = a.SpeciesThreshold.MaxValue,
                });
        }
    }
}
