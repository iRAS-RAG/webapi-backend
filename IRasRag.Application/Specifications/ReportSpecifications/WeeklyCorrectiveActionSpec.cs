using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.ReportSpecifications
{
    /// <summary>
    /// Projects CorrectiveAction entities within a timestamp range and linked to alerts
    /// in the given fish tanks to WeeklyCorrectiveActionItem.
    /// Ordered by most-recent first; caller may further trim the list.
    /// </summary>
    public class WeeklyCorrectiveActionSpec : Specification<CorrectiveAction, WeeklyCorrectiveActionItem>
    {
        public WeeklyCorrectiveActionSpec(DateTime from, DateTime to, IEnumerable<Guid> tankIds)
        {
            Query
                .AsNoTracking()
                .Where(ca => ca.Timestamp >= from && ca.Timestamp <= to
                             && tankIds.Contains(ca.Alert.FishTankId))
                .OrderByDescending(ca => ca.Timestamp)
                .Select(ca => new WeeklyCorrectiveActionItem
                {
                    Id = ca.Id,
                    AlertId = ca.AlertId,
                    ActionTaken = ca.ActionTaken,
                    Notes = ca.Notes ?? string.Empty,
                    PerformedBy = ca.User != null
                        ? (ca.User.FirstName + " " + ca.User.LastName).Trim()
                        : string.Empty,
                    Timestamp = ca.Timestamp,
                });
        }
    }
}
