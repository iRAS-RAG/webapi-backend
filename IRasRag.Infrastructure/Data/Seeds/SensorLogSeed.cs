using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class SensorLogSeed
    {
        // Base date: Jan 1 2026 UTC — all periods are 4-hour windows across one day.
        private static readonly DateTime SeedBase = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // ── Temperature Sensor 1 — 6 × 4h periods ────────────────────────────
        public static readonly Guid TempLog1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001401");
        public static readonly Guid TempLog2Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001402");
        public static readonly Guid TempLog3Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001404");
        public static readonly Guid TempLog4Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001405");
        public static readonly Guid TempLog5Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001406");
        public static readonly Guid TempLog6Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001407");

        // ── Temperature Sensor 2 — 1 period ──────────────────────────────────
        public static readonly Guid TempLog7Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001408");

        // ── pH Sensor 1 — 6 × 4h periods ─────────────────────────────────────
        public static readonly Guid PhLog1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001403");
        public static readonly Guid PhLog2Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001409");
        public static readonly Guid PhLog3Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001410");
        public static readonly Guid PhLog4Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001411");
        public static readonly Guid PhLog5Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001412");
        public static readonly Guid PhLog6Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001413");

        // ── Dissolved Oxygen Sensor 1 — 1 period ─────────────────────────────
        public static readonly Guid DoLog1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001414");

        public static List<SensorLog> SensorLogs =>
            [
                // ── Temperature Sensor 1 ──────────────────────────────────────
                new SensorLog
                {
                    Id = TempLog1Id,
                    SensorId = SensorSeed.TemperatureSensor1Id,
                    PeriodStart = SeedBase, // 00:00–04:00
                    Average = 27.9,
                    Min = 27.5,
                    Max = 28.4,
                    SampleCount = 8,
                    HasWarning = false,
                    CreatedAt = SeedBase.AddHours(4),
                },
                new SensorLog
                {
                    Id = TempLog2Id,
                    SensorId = SensorSeed.TemperatureSensor1Id,
                    PeriodStart = SeedBase.AddHours(4), // 04:00–08:00
                    Average = 28.6,
                    Min = 28.1,
                    Max = 29.1,
                    SampleCount = 8,
                    HasWarning = false,
                    CreatedAt = SeedBase.AddHours(8),
                },
                new SensorLog
                {
                    Id = TempLog3Id,
                    SensorId = SensorSeed.TemperatureSensor1Id,
                    PeriodStart = SeedBase.AddHours(8), // 08:00–12:00
                    Average = 29.8,
                    Min = 29.1,
                    Max = 30.4,
                    SampleCount = 8,
                    HasWarning = false,
                    CreatedAt = SeedBase.AddHours(12),
                },
                new SensorLog
                {
                    Id = TempLog4Id,
                    SensorId = SensorSeed.TemperatureSensor1Id,
                    PeriodStart = SeedBase.AddHours(12), // 12:00–16:00  ← warning period
                    Average = 31.1,
                    Min = 30.5,
                    Max = 31.8,
                    SampleCount = 8,
                    HasWarning = true,
                    CreatedAt = SeedBase.AddHours(16),
                },
                new SensorLog
                {
                    Id = TempLog5Id,
                    SensorId = SensorSeed.TemperatureSensor1Id,
                    PeriodStart = SeedBase.AddHours(16), // 16:00–20:00
                    Average = 30.3,
                    Min = 29.8,
                    Max = 30.9,
                    SampleCount = 8,
                    HasWarning = false,
                    CreatedAt = SeedBase.AddHours(20),
                },
                new SensorLog
                {
                    Id = TempLog6Id,
                    SensorId = SensorSeed.TemperatureSensor1Id,
                    PeriodStart = SeedBase.AddHours(20), // 20:00–24:00
                    Average = 29.0,
                    Min = 28.6,
                    Max = 29.5,
                    SampleCount = 8,
                    HasWarning = false,
                    CreatedAt = SeedBase.AddHours(24),
                },
                // ── pH Sensor 1 ───────────────────────────────────────────────
                new SensorLog
                {
                    Id = PhLog1Id,
                    SensorId = SensorSeed.PhSensor1Id,
                    PeriodStart = SeedBase, // 00:00–04:00
                    Average = 7.1,
                    Min = 7.0,
                    Max = 7.2,
                    SampleCount = 8,
                    HasWarning = false,
                    CreatedAt = SeedBase.AddHours(4),
                },
                new SensorLog
                {
                    Id = PhLog2Id,
                    SensorId = SensorSeed.PhSensor1Id,
                    PeriodStart = SeedBase.AddHours(4), // 04:00–08:00
                    Average = 7.3,
                    Min = 7.2,
                    Max = 7.4,
                    SampleCount = 8,
                    HasWarning = false,
                    CreatedAt = SeedBase.AddHours(8),
                },
                new SensorLog
                {
                    Id = PhLog3Id,
                    SensorId = SensorSeed.PhSensor1Id,
                    PeriodStart = SeedBase.AddHours(8), // 08:00–12:00
                    Average = 7.5,
                    Min = 7.4,
                    Max = 7.6,
                    SampleCount = 8,
                    HasWarning = false,
                    CreatedAt = SeedBase.AddHours(12),
                },
                new SensorLog
                {
                    Id = PhLog4Id,
                    SensorId = SensorSeed.PhSensor1Id,
                    PeriodStart = SeedBase.AddHours(12), // 12:00–16:00
                    Average = 7.4,
                    Min = 7.3,
                    Max = 7.5,
                    SampleCount = 8,
                    HasWarning = false,
                    CreatedAt = SeedBase.AddHours(16),
                },
                new SensorLog
                {
                    Id = PhLog5Id,
                    SensorId = SensorSeed.PhSensor1Id,
                    PeriodStart = SeedBase.AddHours(16), // 16:00–20:00
                    Average = 7.6,
                    Min = 7.5,
                    Max = 7.7,
                    SampleCount = 8,
                    HasWarning = false,
                    CreatedAt = SeedBase.AddHours(20),
                },
                new SensorLog
                {
                    Id = PhLog6Id,
                    SensorId = SensorSeed.PhSensor1Id,
                    PeriodStart = SeedBase.AddHours(20), // 20:00–24:00
                    Average = 7.2,
                    Min = 7.1,
                    Max = 7.3,
                    SampleCount = 8,
                    HasWarning = false,
                    CreatedAt = SeedBase.AddHours(24),
                },
                // ── Dissolved Oxygen Sensor 1 ─────────────────────────────────
                new SensorLog
                {
                    Id = DoLog1Id,
                    SensorId = SensorSeed.DoSensor1Id,
                    PeriodStart = SeedBase.AddHours(20), // 20:00–24:00
                    Average = 6.8,
                    Min = 6.5,
                    Max = 7.1,
                    SampleCount = 8,
                    HasWarning = false,
                    CreatedAt = SeedBase.AddHours(24),
                },
            ];
    }
}
