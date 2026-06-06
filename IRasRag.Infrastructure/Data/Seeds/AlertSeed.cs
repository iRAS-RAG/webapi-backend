using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class AlertSeed
    {
        private static readonly DateTime SeedTimestamp = new(
            2024,
            01,
            01,
            0,
            0,
            0,
            DateTimeKind.Utc
        );

        public static readonly Guid Alert1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001801");

        public static readonly Guid Alert2Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001802");

        public static readonly Guid Alert3Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001803");

        public static List<Alert> Alerts =>
            [
                // Temperature spike during Fry stage — resolved same day
                new Alert
                {
                    Id = Alert1Id,
                    SensorId = SensorSeed.TemperatureSensor1Id,
                    SpeciesThresholdId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000501"),
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FishTankId = FishTankSeed.TankAId,
                    SensorTypeId = SensorTypeSeed.TemperatureSensorTypeId,
                    TriggerValue = 31.2,
                    RaisedAt = new DateTime(2025, 08, 10, 14, 30, 0, DateTimeKind.Utc),
                    ResolvedAt = new DateTime(2025, 08, 10, 18, 0, 0, DateTimeKind.Utc),
                    Status = AlertStatus.RESOLVED,
                    CreatedAt = SeedTimestamp,
                },
                // Mild pH drift during Fry — resolved after 2.5 hours
                new Alert
                {
                    Id = Alert2Id,
                    SensorId = SensorSeed.PhSensor1Id,
                    SpeciesThresholdId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000502"),
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FishTankId = FishTankSeed.TankAId,
                    SensorTypeId = SensorTypeSeed.PhSensorTypeId,
                    TriggerValue = 8.4,
                    RaisedAt = new DateTime(2025, 08, 14, 8, 0, 0, DateTimeKind.Utc),
                    ResolvedAt = new DateTime(2025, 08, 14, 10, 30, 0, DateTimeKind.Utc),
                    Status = AlertStatus.RESOLVED,
                    CreatedAt = SeedTimestamp,
                },
                // Temperature alert during Fingerling — acknowledged, not yet resolved
                new Alert
                {
                    Id = Alert3Id,
                    SensorId = SensorSeed.TemperatureSensor1Id,
                    SpeciesThresholdId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000509"),
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FishTankId = FishTankSeed.TankAId,
                    SensorTypeId = SensorTypeSeed.TemperatureSensorTypeId,
                    TriggerValue = 30.2,
                    RaisedAt = new DateTime(2025, 09, 05, 12, 0, 0, DateTimeKind.Utc),
                    ResolvedAt = new DateTime(2025, 09, 05, 13, 0, 0, DateTimeKind.Utc),
                    Status = AlertStatus.RESOLVED,
                    CreatedAt = SeedTimestamp,
                },
            ];
    }
}
