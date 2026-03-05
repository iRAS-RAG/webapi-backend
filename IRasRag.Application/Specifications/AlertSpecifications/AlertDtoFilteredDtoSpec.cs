using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.AlertSpecifications
{
    public class AlertDtoFilteredDtoSpec : Specification<Alert, AlertDto>
    {
        public AlertDtoFilteredDtoSpec(Expression<Func<Alert, bool>> predicate)
        {
            Query
                .AsNoTracking()
                .Where(predicate)
                .Select(a => new AlertDto
                {
                    Id = a.Id,
                    SensorLogId = a.SensorLogId,
                    SpeciesThresholdId = a.SpeciesThresholdId,
                    FarmingBatchId = a.FarmingBatchId,
                    FarmingBatchName = a.FarmingBatch != null ? a.FarmingBatch.Name : null,
                    FishTankId = a.FishTankId,
                    FishTankName = a.FishTank.Name,
                    SensorTypeName = a.SensorType.Name,
                    Value = a.Value,
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
