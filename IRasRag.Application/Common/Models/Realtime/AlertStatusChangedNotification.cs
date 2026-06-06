namespace IRasRag.Application.Common.Models.Realtime
{
    public record AlertStatusChangedNotification(Guid AlertId, Guid TankId, string NewStatus);
}
