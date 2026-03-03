using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.AnalyticsSpecifications
{
    /// <summary>
    /// Projects Alert entities within a date range to a lightweight AlertFrequencyProjection.
    /// Always scoped to <paramref name="userTankIds"/> for data isolation.
    /// Supports optional further filtering by a specific FishTankId or FarmId.
    /// Used by the F12 alert-frequency endpoint to avoid loading full entity graphs.
    /// </summary>
    public class AlertFrequencySpec : Specification<Alert, AlertFrequencyProjection>
    {
        public AlertFrequencySpec(
            DateTime from,
            DateTime to,
            IEnumerable<Guid> userTankIds,
            Guid? fishTankId,
            Guid? farmId)
        {
            var tankSet = userTankIds is HashSet<Guid> hs ? hs : userTankIds.ToHashSet();

            Query
                .AsNoTracking()
                .Where(a => tankSet.Contains(a.FishTankId) && a.RaisedAt >= from && a.RaisedAt <= to);

            if (fishTankId.HasValue)
                Query.Where(a => a.FishTankId == fishTankId.Value);

            if (farmId.HasValue)
                Query.Where(a => a.FishTank.FarmId == farmId.Value);

            Query.Select(a => new AlertFrequencyProjection
            {
                SensorTypeId = a.SensorTypeId,
                FishTankId = a.FishTankId,
                FishTankName = a.FishTank.Name,
                StatusStr = a.Status.ToString(),
                RaisedAt = a.RaisedAt,
                ResolvedAt = a.ResolvedAt,
            });
        }
    }
}
