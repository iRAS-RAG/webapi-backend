using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class AlertSeed
    {
        private static readonly DateTime SeedTimestamp = new DateTime(
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
            new()
            {
                new Alert
                {
                    Id = Alert1Id,
                    SensorLogId = SensorLogSeed.TempLog2Id,
                    SpeciesThresholdId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000501"),
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FishTankId = FishTankSeed.TankAId,
                    SensorTypeId = SensorTypeSeed.TemperatureSensorTypeId,
                    Value = 31.2f,
                    RaisedAt = new DateTime(2024, 01, 15, 14, 30, 0, DateTimeKind.Utc),
                    ResolvedAt = null,
                    Status = AlertStatus.OPEN,
                    CreatedAt = SeedTimestamp,
                },
                new Alert
                {
                    Id = Alert2Id,
                    SensorLogId = SensorLogSeed.PhLog1Id,
                    SpeciesThresholdId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000502"),
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FishTankId = FishTankSeed.TankAId,
                    SensorTypeId = SensorTypeSeed.PhSensorTypeId,
                    Value = 7.2f,
                    RaisedAt = new DateTime(2024, 01, 16, 8, 0, 0, DateTimeKind.Utc),
                    ResolvedAt = new DateTime(2024, 01, 16, 10, 30, 0, DateTimeKind.Utc),
                    Status = AlertStatus.RESOLVED,
                    CreatedAt = SeedTimestamp,
                },
                new Alert
                {
                    Id = Alert3Id,
                    SensorLogId = SensorLogSeed.TempLog1Id,
                    SpeciesThresholdId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000501"),
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FishTankId = FishTankSeed.TankAId,
                    SensorTypeId = SensorTypeSeed.TemperatureSensorTypeId,
                    Value = 28.5f,
                    RaisedAt = new DateTime(2024, 01, 17, 12, 0, 0, DateTimeKind.Utc),
                    ResolvedAt = null,
                    Status = AlertStatus.ACKNOWLEDGED,
                    CreatedAt = SeedTimestamp,
                },
            };
    }
}
