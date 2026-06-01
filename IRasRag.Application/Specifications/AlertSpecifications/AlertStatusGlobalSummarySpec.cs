using Ardalis.Specification;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;

namespace IRasRag.Application.Specifications.AlertSpecifications
{
    public class AlertStatusGlobalSummarySpec : Specification<Alert, AlertStatus>
    {
        public AlertStatusGlobalSummarySpec()
        {
            Query.AsNoTracking().Select(a => a.Status);
        }
    }
}
