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
                ["value"] = a => a.Value,
                ["fishtankname"] = a => a.FishTank.Name,
            };

            ApplySearch(
                request.SearchTerm,
                [
                    a => a.FishTank.Name,
                    a => a.SensorType.Name,
                    a => a.FarmingBatch != null ? a.FarmingBatch.Name : null,
                ]
            );

            ApplyFilter(request.Status, a => a.Status == request.Status);

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "raisedat");

            Query.Select(a => new AlertDto
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
