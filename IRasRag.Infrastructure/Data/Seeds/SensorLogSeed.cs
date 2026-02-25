using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class SensorLogSeed
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

        public static readonly Guid TempLog1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001401");

        public static readonly Guid TempLog2Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001402");

        public static readonly Guid PhLog1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001403");

        public static readonly Guid DoLog1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001404");

        public static List<SensorLog> SensorLogs =>
            new()
            {
                new SensorLog
                {
                    Id = TempLog1Id,
                    SensorId = SensorSeed.TemperatureSensor1Id,
                    Data = 28.5,
                    IsWarning = false,
                    DataJson = "{\"temperature\": 28.5, \"unit\": \"C\"}",
                    CreatedAt = SeedTimestamp,
                },
                new SensorLog
                {
                    Id = TempLog2Id,
                    SensorId = SensorSeed.TemperatureSensor1Id,
                    Data = 31.2,
                    IsWarning = true,
                    DataJson = "{\"temperature\": 31.2, \"unit\": \"C\"}",
                    CreatedAt = SeedTimestamp.AddMinutes(30),
                },
                new SensorLog
                {
                    Id = PhLog1Id,
                    SensorId = SensorSeed.PhSensor1Id,
                    Data = 7.2,
                    IsWarning = false,
                    DataJson = "{\"ph\": 7.2}",
                    CreatedAt = SeedTimestamp,
                },
                new SensorLog
                {
                    Id = DoLog1Id,
                    SensorId = SensorSeed.DoSensor1Id,
                    Data = 6.8,
                    IsWarning = false,
                    DataJson = "{\"dissolvedOxygen\": 6.8, \"unit\": \"mg/L\"}",
                    CreatedAt = SeedTimestamp,
                },
            };
    }
}
