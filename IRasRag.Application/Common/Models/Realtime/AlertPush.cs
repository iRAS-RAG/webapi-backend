namespace IRasRag.Application.Common.Models.Realtime
{
    public record AlertPush(
        Guid AlertId,
        Guid TankId,
        string SensorName,
        string? BatchName,
        double TriggerValue,
        double MinValue,
        double MaxValue,
        DateTime RaisedAt);
}
