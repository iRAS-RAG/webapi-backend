using Ardalis.Specification;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;

namespace IRasRag.Application.Specifications.ReportSpecifications
{
    public class AlertStatusCountSpec : Specification<Alert, AlertStatus>
    {
        public AlertStatusCountSpec(DateTime from, DateTime to, IEnumerable<Guid> tankIds)
        {
            Query
                .AsNoTracking()
                .Where(a => tankIds.Contains(a.FishTankId) && a.RaisedAt >= from && a.RaisedAt < to)
                .Select(a => a.Status);
        }
    }
}
