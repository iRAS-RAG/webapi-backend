using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class MortalityLogSeed
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

        // ── Existing IDs (preserved) ────────────────────────────────────
        public static readonly Guid MortalityLog1Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001701"
        );
        public static readonly Guid MortalityLog2Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001702"
        );

        // ── Batch 1 mortality IDs (additional) ─────────────────────────
        // Fry stage
        public static readonly Guid MortalityLog1_3Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001703"
        );
        public static readonly Guid MortalityLog1_4Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001704"
        );
        public static readonly Guid MortalityLog1_5Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001705"
        );

        // Fingerling stage
        public static readonly Guid MortalityLog1_6Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001706"
        );

        // Juvenile stage
        public static readonly Guid MortalityLog1_7Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001707"
        );

        // Grow-out stage
        public static readonly Guid MortalityLog1_8Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001708"
        );
        public static readonly Guid MortalityLog1_9Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001709"
        );
        public static readonly Guid MortalityLog1_10Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001710"
        );

        // ── Batch 2 mortality IDs ──────────────────────────────────────
        // Fry stage
        public static readonly Guid MortalityLog2_1Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001711"
        );
        public static readonly Guid MortalityLog2_2Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001712"
        );
        public static readonly Guid MortalityLog2_3Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001713"
        );

        // Fingerling stage
        public static readonly Guid MortalityLog2_4Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001714"
        );
        public static readonly Guid MortalityLog2_5Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001715"
        );
        public static readonly Guid MortalityLog2_6Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001716"
        );

        // Juvenile stage (partial)
        public static readonly Guid MortalityLog2_7Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001717"
        );
        public static readonly Guid MortalityLog2_8Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001718"
        );

        public static List<MortalityLog> MortalityLogs =>
            [
                // ═════════════════════════════════════════════════════════════
                // BATCH 1 — Full cycle: Fry → Fingerling → Juvenile → Grow-out
                // Start: 2025-08-01  |  Harvest: 2026-02-20  |  1000 → 825 fish
                // Total losses: 175 fish (across all stages)
                // ═════════════════════════════════════════════════════════════

                // ── Fry stage (Aug 1–22) — 80 deaths expected ──────────
                // Average weight: ~2 g/fish (0.002 kg)
                // Early fry — handling stress, weak fry don't survive
                new MortalityLog
                {
                    Id = MortalityLog1_3Id,
                    BatchId = FarmingBatchSeed.Batch1Id,
                    UserId = UserSeed.OperatorId,
                    Quantity = 25,
                    LostWeightKg = 0.050, // 25 × ~0.002 kg
                    Date = new DateTime(2025, 08, 05, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // Mid fry — natural attrition, water quality adjustment
                new MortalityLog
                {
                    Id = MortalityLog1_4Id,
                    BatchId = FarmingBatchSeed.Batch1Id,
                    UserId = UserSeed.OperatorId,
                    Quantity = 28,
                    LostWeightKg = 0.056, // 28 × ~0.002 kg
                    Date = new DateTime(2025, 08, 12, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // Late fry — grading, some runts culled
                new MortalityLog
                {
                    Id = MortalityLog1_5Id,
                    BatchId = FarmingBatchSeed.Batch1Id,
                    UserId = UserSeed.OperatorId,
                    Quantity = 27,
                    LostWeightKg = 0.054, // 27 × ~0.002 kg
                    Date = new DateTime(2025, 08, 20, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // ── Fingerling stage (Aug 22 – Sep 20) — 46 deaths ────
                // Average weight: ~15 g/fish (0.015 kg)
                // (pre-existing entry — mid fingerling spike likely from water quality)
                new MortalityLog
                {
                    Id = MortalityLog1Id,
                    BatchId = FarmingBatchSeed.Batch1Id,
                    UserId = UserSeed.OperatorId,
                    Quantity = 30,
                    LostWeightKg = 0.45, // 30 × ~0.015 kg
                    Date = new DateTime(2025, 09, 10, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // Early fingerling — transition stress from starter to grower feed
                new MortalityLog
                {
                    Id = MortalityLog1_6Id,
                    BatchId = FarmingBatchSeed.Batch1Id,
                    UserId = UserSeed.OperatorId,
                    Quantity = 16,
                    LostWeightKg = 0.24, // 16 × ~0.015 kg
                    Date = new DateTime(2025, 08, 27, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // ── Juvenile stage (Sep 21 – Nov 21) — 26 deaths ──────
                // Average weight: ~100 g/fish (0.100 kg)
                // (pre-existing entry)
                new MortalityLog
                {
                    Id = MortalityLog2Id,
                    BatchId = FarmingBatchSeed.Batch1Id,
                    UserId = UserSeed.OperatorId,
                    Quantity = 20,
                    LostWeightKg = 2.0, // 20 × ~0.100 kg
                    Date = new DateTime(2025, 10, 25, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // Late juvenile — pre-winter temperature fluctuation
                new MortalityLog
                {
                    Id = MortalityLog1_7Id,
                    BatchId = FarmingBatchSeed.Batch1Id,
                    UserId = UserSeed.OperatorId,
                    Quantity = 6,
                    LostWeightKg = 0.6, // 6 × ~0.100 kg
                    Date = new DateTime(2025, 11, 10, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // ── Grow-out stage (Nov 22 – Feb 20) — 23 deaths ─────
                // Average weight: ~300–500 g/fish as they grow
                new MortalityLog
                {
                    Id = MortalityLog1_8Id,
                    BatchId = FarmingBatchSeed.Batch1Id,
                    UserId = UserSeed.OperatorId,
                    Quantity = 8,
                    LostWeightKg = 2.4, // 8 × ~0.300 kg
                    Date = new DateTime(2025, 12, 08, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // Mid grow-out — cold spell losses
                new MortalityLog
                {
                    Id = MortalityLog1_9Id,
                    BatchId = FarmingBatchSeed.Batch1Id,
                    UserId = UserSeed.OperatorId,
                    Quantity = 6,
                    LostWeightKg = 2.4, // 6 × ~0.400 kg
                    Date = new DateTime(2026, 01, 15, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // Pre-harvest — handling during sample weighing
                new MortalityLog
                {
                    Id = MortalityLog1_10Id,
                    BatchId = FarmingBatchSeed.Batch1Id,
                    UserId = UserSeed.OperatorId,
                    Quantity = 3,
                    LostWeightKg = 1.5, // 3 × ~0.500 kg (near-market weight)
                    Date = new DateTime(2026, 02, 08, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // ═════════════════════════════════════════════════════════════
                // BATCH 2 — Early harvest: Fry → Fingerling → Juvenile (partial)
                // Start: 2026-03-01  |  Harvest: 2026-05-25  |  800 → 700 fish
                // Total losses: 100 fish
                // ═════════════════════════════════════════════════════════════

                // ── Fry stage (Mar 1–22) — 60 deaths ───────────────────
                // Average weight: ~2 g/fish (0.002 kg)
                new MortalityLog
                {
                    Id = MortalityLog2_1Id,
                    BatchId = FarmingBatchSeed.Batch2Id,
                    UserId = UserSeed.OperatorId,
                    Quantity = 20,
                    LostWeightKg = 0.040, // 20 × ~0.002 kg
                    Date = new DateTime(2026, 03, 05, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new MortalityLog
                {
                    Id = MortalityLog2_2Id,
                    BatchId = FarmingBatchSeed.Batch2Id,
                    UserId = UserSeed.OperatorId,
                    Quantity = 20,
                    LostWeightKg = 0.040,
                    Date = new DateTime(2026, 03, 12, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new MortalityLog
                {
                    Id = MortalityLog2_3Id,
                    BatchId = FarmingBatchSeed.Batch2Id,
                    UserId = UserSeed.OperatorId,
                    Quantity = 20,
                    LostWeightKg = 0.040,
                    Date = new DateTime(2026, 03, 20, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // ── Fingerling stage (Mar 22 – Apr 21) — 30 deaths ────
                // Average weight: ~15 g/fish (0.015 kg)
                new MortalityLog
                {
                    Id = MortalityLog2_4Id,
                    BatchId = FarmingBatchSeed.Batch2Id,
                    UserId = UserSeed.OperatorId,
                    Quantity = 10,
                    LostWeightKg = 0.15, // 10 × ~0.015 kg
                    Date = new DateTime(2026, 03, 28, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new MortalityLog
                {
                    Id = MortalityLog2_5Id,
                    BatchId = FarmingBatchSeed.Batch2Id,
                    UserId = UserSeed.OperatorId,
                    Quantity = 10,
                    LostWeightKg = 0.15,
                    Date = new DateTime(2026, 04, 08, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new MortalityLog
                {
                    Id = MortalityLog2_6Id,
                    BatchId = FarmingBatchSeed.Batch2Id,
                    UserId = UserSeed.OperatorId,
                    Quantity = 10,
                    LostWeightKg = 0.15,
                    Date = new DateTime(2026, 04, 18, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // ── Juvenile stage (partial, Apr 21 – May 25) — 10 deaths ──
                // Average weight: ~100 g/fish (0.100 kg)
                new MortalityLog
                {
                    Id = MortalityLog2_7Id,
                    BatchId = FarmingBatchSeed.Batch2Id,
                    UserId = UserSeed.OperatorId,
                    Quantity = 6,
                    LostWeightKg = 0.6, // 6 × ~0.100 kg
                    Date = new DateTime(2026, 05, 05, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new MortalityLog
                {
                    Id = MortalityLog2_8Id,
                    BatchId = FarmingBatchSeed.Batch2Id,
                    UserId = UserSeed.OperatorId,
                    Quantity = 4,
                    LostWeightKg = 0.4, // 4 × ~0.100 kg
                    Date = new DateTime(2026, 05, 20, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
            ];
    }
}
