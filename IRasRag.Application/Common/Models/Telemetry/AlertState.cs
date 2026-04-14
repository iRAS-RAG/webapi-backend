using IRasRag.Domain.Enums;

namespace IRasRag.Application.Common.Models.Telemetry
{
    public class AlertState
    {
        public Guid? ActiveAlertId;
        public int BreachCount = 0;
        public int ResolveCount = 0;
        public double LastValue;
    }
}
