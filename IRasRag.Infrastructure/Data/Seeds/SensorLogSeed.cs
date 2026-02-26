using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class SensorLogSeed
    {
        // Base date: fixed at midnight UTC for consistent manual testing.
        private static readonly DateTime SeedBase = new DateTime(
            2026,
            1,
            1,
            0,
            0,
            0,
            DateTimeKind.Utc
        );

        public static readonly Guid TempLog1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001401");
        public static readonly Guid TempLog2Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001402");
        public static readonly Guid PhLog1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001403");

        // Additional entries – one per 4-hour slot, two sensors
        public static readonly Guid TempLog3Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001404");
        public static readonly Guid TempLog4Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001405");
        public static readonly Guid TempLog5Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001406");
        public static readonly Guid TempLog6Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001407");
        public static readonly Guid TempLog7Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001408");
        public static readonly Guid PhLog2Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001409");
        public static readonly Guid PhLog3Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001410");
        public static readonly Guid PhLog4Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001411");
        public static readonly Guid PhLog5Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001412");
        public static readonly Guid PhLog6Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001413");
        public static readonly Guid DoLog1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001414");

        public static List<SensorLog> SensorLogs =>
            new()
            {
                // ── Slot 00:00–04:00 ──────────────────────────────────────────
                new SensorLog
                {
                    Id = TempLog1Id,
                    SensorId = SensorSeed.TemperatureSensor1Id,
                    Data = 27.8,
                    IsWarning = false,
                    DataJson = "{\"temperature\": 27.8, \"unit\": \"C\"}",
                    CreatedAt = SeedBase.AddHours(1),
                },
                new SensorLog
                {
                    Id = TempLog2Id,
                    SensorId = SensorSeed.TemperatureSensor1Id,
                    Data = 28.2,
                    IsWarning = false,
                    DataJson = "{\"temperature\": 28.2, \"unit\": \"C\"}",
                    CreatedAt = SeedBase.AddHours(2),
                },
                new SensorLog
                {
                    Id = PhLog1Id,
                    SensorId = SensorSeed.PhSensor1Id,
                    Data = 7.1,
                    IsWarning = false,
                    DataJson = "{\"ph\": 7.1}",
                    CreatedAt = SeedBase.AddHours(1),
                },
                // ── Slot 04:00–08:00 ──────────────────────────────────────────
                new SensorLog
                {
                    Id = TempLog3Id,
                    SensorId = SensorSeed.TemperatureSensor1Id,
                    Data = 28.6,
                    IsWarning = false,
                    DataJson = "{\"temperature\": 28.6, \"unit\": \"C\"}",
                    CreatedAt = SeedBase.AddHours(5),
                },
                new SensorLog
                {
                    Id = PhLog2Id,
                    SensorId = SensorSeed.PhSensor1Id,
                    Data = 7.3,
                    IsWarning = false,
                    DataJson = "{\"ph\": 7.3}",
                    CreatedAt = SeedBase.AddHours(5),
                },
                // ── Slot 08:00–12:00 ──────────────────────────────────────────
                new SensorLog
                {
                    Id = TempLog4Id,
                    SensorId = SensorSeed.TemperatureSensor1Id,
                    Data = 29.4,
                    IsWarning = false,
                    DataJson = "{\"temperature\": 29.4, \"unit\": \"C\"}",
                    CreatedAt = SeedBase.AddHours(9),
                },
                new SensorLog
                {
                    Id = PhLog3Id,
                    SensorId = SensorSeed.PhSensor1Id,
                    Data = 7.5,
                    IsWarning = false,
                    DataJson = "{\"ph\": 7.5}",
                    CreatedAt = SeedBase.AddHours(9),
                },
                // ── Slot 12:00–16:00 ──────────────────────────────────────────
                new SensorLog
                {
                    Id = TempLog5Id,
                    SensorId = SensorSeed.TemperatureSensor1Id,
                    Data = 31.2,
                    IsWarning = true,
                    DataJson = "{\"temperature\": 31.2, \"unit\": \"C\"}",
                    CreatedAt = SeedBase.AddHours(13),
                },
                new SensorLog
                {
                    Id = PhLog4Id,
                    SensorId = SensorSeed.PhSensor1Id,
                    Data = 7.4,
                    IsWarning = false,
                    DataJson = "{\"ph\": 7.4}",
                    CreatedAt = SeedBase.AddHours(13),
                },
                // ── Slot 16:00–20:00 ──────────────────────────────────────────
                new SensorLog
                {
                    Id = TempLog6Id,
                    SensorId = SensorSeed.TemperatureSensor1Id,
                    Data = 30.5,
                    IsWarning = false,
                    DataJson = "{\"temperature\": 30.5, \"unit\": \"C\"}",
                    CreatedAt = SeedBase.AddHours(17),
                },
                new SensorLog
                {
                    Id = PhLog5Id,
                    SensorId = SensorSeed.PhSensor1Id,
                    Data = 7.6,
                    IsWarning = false,
                    DataJson = "{\"ph\": 7.6}",
                    CreatedAt = SeedBase.AddHours(17),
                },
                // ── Slot 20:00–24:00 ──────────────────────────────────────────
                new SensorLog
                {
                    Id = TempLog7Id,
                    SensorId = SensorSeed.TemperatureSensor1Id,
                    Data = 29.1,
                    IsWarning = false,
                    DataJson = "{\"temperature\": 29.1, \"unit\": \"C\"}",
                    CreatedAt = SeedBase.AddHours(21),
                },
                new SensorLog
                {
                    Id = PhLog6Id,
                    SensorId = SensorSeed.PhSensor1Id,
                    Data = 7.2,
                    IsWarning = false,
                    DataJson = "{\"ph\": 7.2}",
                    CreatedAt = SeedBase.AddHours(21),
                },
                new SensorLog
                {
                    Id = DoLog1Id,
                    SensorId = SensorSeed.DoSensor1Id,
                    Data = 6.8,
                    IsWarning = false,
                    DataJson = "{\"dissolvedOxygen\": 6.8, \"unit\": \"mg/L\"}",
                    CreatedAt = SeedBase.AddHours(21),
                },
            };
    }
}
