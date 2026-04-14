using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.AlertSpecifications
{
    public class AlertDtoListSpec : BaseListSpec<Alert, AlertDto>
    {
        public AlertDtoListSpec(AlertListRequest request)
        {
            Query.AsNoTracking();

            var sortMap = new Dictionary<string, Expression<Func<Alert, object?>>>
            {
                ["raisedat"] = a => a.RaisedAt,
                ["status"] = a => a.Status,
                ["value"] = a => a.TriggerValue,
                ["fishtankname"] = a => a.FishTank.Name,
                ["sensorname"] = a => a.Sensor.Name,
            };

            ApplySearch(
                request.SearchTerm,
                [
                    a => a.FishTank.Name,
                    a => a.SensorType.Name,
                    a => a.Sensor.Name,
                    a => a.FarmingBatch != null ? a.FarmingBatch.Name : null,
                ]
            );

            ApplyFilter(request.Status, a => a.Status == request.Status);
            ApplyFilter(request.TankId, a => a.FishTankId == request.TankId);
            ApplyFilter(request.SensorId, a => a.SensorId == request.SensorId);

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "raisedat");

            Query.Select(a => new AlertDto
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
