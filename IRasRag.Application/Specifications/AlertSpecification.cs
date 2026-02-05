using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications
{
    public sealed class AlertDtoListSpec : Specification<Alert, AlertDto>
    {
        public AlertDtoListSpec()
        {
            Query
                .AsNoTracking()
                .Include(a => a.FishTank)
                .Include(a => a.SensorType)
                .Include(a => a.FarmingBatch)
                .Select(a => new AlertDto
                {
                    Id = a.Id,
                    SensorLogId = a.SensorLogId,
                    SpeciesThresholdId = a.SpeciesThresholdId,
                    FarmingBatchId = a.FarmingBatchId,
                    FarmingBatchName = a.FarmingBatch != null ? a.FarmingBatch.Name : null,
                    FishTankId = a.FishTankId,
                    FishTankName = a.FishTank.Name,
                    SensorTypeId = a.SensorTypeId,
                    SensorTypeName = a.SensorType.Name,
                    Value = a.Value,
                    RaisedAt = a.RaisedAt,
                    ResolvedAt = a.ResolvedAt,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt,
                    ModifiedAt = a.ModifiedAt,
                });
        }
    }
}
