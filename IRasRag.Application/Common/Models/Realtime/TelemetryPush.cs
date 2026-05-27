namespace IRasRag.Application.Common.Models.Realtime
{
    public record TelemetryPush(
        Guid SensorId,
        Guid TankId,
        double Value,
        DateTime Timestamp,
        string? SensorTypeName
    );
}
