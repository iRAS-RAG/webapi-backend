using Ardalis.Specification;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.ReportSpecifications
{
    /// <summary>
    /// Projects Alert entities within a date range and belonging to the given fish tanks
    /// to their SensorType name, already grouped and counted.
    /// Used to compute the alert-type breakdown for the weekly report.
    /// </summary>
    public class WeeklyAlertSensorTypeSpec : Specification<Alert, string>
    {
        public WeeklyAlertSensorTypeSpec(DateTime from, DateTime to, IEnumerable<Guid> tankIds)
        {
            Query
                .AsNoTracking()
                .Where(a => a.RaisedAt >= from && a.RaisedAt < to && tankIds.Contains(a.FishTankId))
                .Select(a => a.SensorType.Name);
        }
    }
}
