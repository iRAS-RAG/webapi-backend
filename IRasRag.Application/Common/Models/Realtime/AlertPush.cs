namespace IRasRag.Application.Common.Models.Realtime
{
    public record AlertPush(
        Guid AlertId,
        Guid TankId,
        string TankName,
        string? SensorTypeName,
        double TriggerValue,
        double MinValue,
        double MaxValue
    );
}
