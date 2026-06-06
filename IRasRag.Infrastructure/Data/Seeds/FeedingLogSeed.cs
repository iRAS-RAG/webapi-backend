using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class FeedingLogSeed
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
        public static readonly Guid FeedLog1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001601");
        public static readonly Guid FeedLog2Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001602");
        public static readonly Guid FeedLog3Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001603");

        // ── Batch 1 feeding IDs (additional) ───────────────────────────
        // Fry stage
        public static readonly Guid FeedLog1_4Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001604"
        );
        public static readonly Guid FeedLog1_5Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001605"
        );
        public static readonly Guid FeedLog1_6Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001606"
        );

        // Fingerling stage
        public static readonly Guid FeedLog1_7Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001607"
        );
        public static readonly Guid FeedLog1_8Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001608"
        );
        public static readonly Guid FeedLog1_9Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001609"
        );
        public static readonly Guid FeedLog1_10Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001610"
        );

        // Juvenile stage
        public static readonly Guid FeedLog1_11Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001611"
        );
        public static readonly Guid FeedLog1_12Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001612"
        );
        public static readonly Guid FeedLog1_13Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001613"
        );
        public static readonly Guid FeedLog1_14Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001614"
        );

        // Grow-out stage
        public static readonly Guid FeedLog1_15Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001615"
        );
        public static readonly Guid FeedLog1_16Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001616"
        );
        public static readonly Guid FeedLog1_17Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001617"
        );
        public static readonly Guid FeedLog1_18Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001618"
        );
        public static readonly Guid FeedLog1_19Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001619"
        );

        // ── Batch 2 feeding IDs ────────────────────────────────────────
        // Fry stage
        public static readonly Guid FeedLog2_1Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001620"
        );
        public static readonly Guid FeedLog2_2Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001621"
        );
        public static readonly Guid FeedLog2_3Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001622"
        );

        // Fingerling stage
        public static readonly Guid FeedLog2_4Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001623"
        );
        public static readonly Guid FeedLog2_5Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001624"
        );
        public static readonly Guid FeedLog2_6Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001625"
        );
        public static readonly Guid FeedLog2_7Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001626"
        );

        // Juvenile stage (partial)
        public static readonly Guid FeedLog2_8Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001627"
        );
        public static readonly Guid FeedLog2_9Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001628"
        );
        public static readonly Guid FeedLog2_10Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001629"
        );
        public static readonly Guid FeedLog2_11Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001630"
        );

        public static List<FeedingLog> FeedingLogs =>
            [
                // ═════════════════════════════════════════════════════════════
                // BATCH 1 — Full cycle: Fry → Fingerling → Juvenile → Grow-out
                // Start: 2025-08-01  |  Harvest: 2026-02-20  |  1000 → 825 fish
                // ═════════════════════════════════════════════════════════════

                // ── Fry stage (Aug 1–22) — Starter feed ─────────────────
                // Feed rate: 0.3 kg/100 fish, 7×/day. At 1000 fish ≈ 3.0 kg per feed
                // (pre-existing entry — morning)
                new FeedingLog
                {
                    Id = FeedLog1Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FeedTypeId = FeedTypeSeed.StarterFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 3.0,
                    CreatedDate = new DateTime(2025, 08, 10, 6, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // (pre-existing entry — midday)
                new FeedingLog
                {
                    Id = FeedLog2Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FeedTypeId = FeedTypeSeed.StarterFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 2.8,
                    CreatedDate = new DateTime(2025, 08, 14, 12, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // Early fry — morning feed
                new FeedingLog
                {
                    Id = FeedLog1_4Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FeedTypeId = FeedTypeSeed.StarterFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 3.0,
                    CreatedDate = new DateTime(2025, 08, 03, 8, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // Late afternoon feed
                new FeedingLog
                {
                    Id = FeedLog1_5Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FeedTypeId = FeedTypeSeed.StarterFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 3.0,
                    CreatedDate = new DateTime(2025, 08, 07, 16, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // Late fry — fish slightly bigger, count slightly lower
                new FeedingLog
                {
                    Id = FeedLog1_6Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FeedTypeId = FeedTypeSeed.StarterFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 2.8,
                    CreatedDate = new DateTime(2025, 08, 18, 10, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // ── Fingerling stage (Aug 22 – Sep 20) — Grower feed ────
                // Feed rate: 1.0 kg/100 fish, 5×/day. At ~920 fish ≈ 9.2 kg per feed
                // (pre-existing entry — evening)
                new FeedingLog
                {
                    Id = FeedLog3Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 9.0,
                    CreatedDate = new DateTime(2025, 09, 05, 18, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // Early fingerling — morning
                new FeedingLog
                {
                    Id = FeedLog1_7Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 9.2,
                    CreatedDate = new DateTime(2025, 08, 24, 7, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // Mid fingerling — midday
                new FeedingLog
                {
                    Id = FeedLog1_8Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 9.0,
                    CreatedDate = new DateTime(2025, 08, 30, 12, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // Mid fingerling — afternoon
                new FeedingLog
                {
                    Id = FeedLog1_9Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 9.0,
                    CreatedDate = new DateTime(2025, 09, 10, 14, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // Late fingerling — morning
                new FeedingLog
                {
                    Id = FeedLog1_10Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 8.7,
                    CreatedDate = new DateTime(2025, 09, 18, 8, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // ── Juvenile stage (Sep 21 – Nov 21) — Grower → Finisher ──
                // Feed rate: 2.5 kg/100 fish, 3×/day. At ~874 fish ≈ 21.8 kg per feed.
                // Early juvenile still on Grower; late juvenile transitions to Finisher.
                new FeedingLog
                {
                    Id = FeedLog1_11Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 21.0,
                    CreatedDate = new DateTime(2025, 09, 28, 7, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new FeedingLog
                {
                    Id = FeedLog1_12Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 21.5,
                    CreatedDate = new DateTime(2025, 10, 12, 12, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // Transition to finisher feed
                new FeedingLog
                {
                    Id = FeedLog1_13Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FeedTypeId = FeedTypeSeed.FinisherFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 21.0,
                    CreatedDate = new DateTime(2025, 10, 28, 17, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new FeedingLog
                {
                    Id = FeedLog1_14Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FeedTypeId = FeedTypeSeed.FinisherFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 21.0,
                    CreatedDate = new DateTime(2025, 11, 10, 8, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // ── Grow-out stage (Nov 22 – Feb 20) — Finisher feed ───
                // Feed rate: 4.0 kg/100 fish, 2×/day. At ~848 fish ≈ 33.9 kg per feed
                new FeedingLog
                {
                    Id = FeedLog1_15Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FeedTypeId = FeedTypeSeed.FinisherFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 33.0,
                    CreatedDate = new DateTime(2025, 11, 30, 7, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new FeedingLog
                {
                    Id = FeedLog1_16Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FeedTypeId = FeedTypeSeed.FinisherFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 33.5,
                    CreatedDate = new DateTime(2025, 12, 20, 12, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new FeedingLog
                {
                    Id = FeedLog1_17Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FeedTypeId = FeedTypeSeed.FinisherFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 33.0,
                    CreatedDate = new DateTime(2026, 01, 08, 8, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new FeedingLog
                {
                    Id = FeedLog1_18Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FeedTypeId = FeedTypeSeed.FinisherFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 33.0,
                    CreatedDate = new DateTime(2026, 01, 25, 16, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new FeedingLog
                {
                    Id = FeedLog1_19Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FeedTypeId = FeedTypeSeed.FinisherFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 32.5,
                    CreatedDate = new DateTime(2026, 02, 10, 7, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // ═════════════════════════════════════════════════════════════
                // BATCH 2 — Early harvest: Fry → Fingerling → Juvenile (partial)
                // Start: 2026-03-01  |  Harvest: 2026-05-25  |  800 → 700 fish
                // ═════════════════════════════════════════════════════════════

                // ── Fry stage (Mar 1–22) — Starter feed ─────────────────
                // Feed rate: 0.3 kg/100 fish, 7×/day. At 800 fish ≈ 2.4 kg per feed
                new FeedingLog
                {
                    Id = FeedLog2_1Id,
                    FarmingBatchId = FarmingBatchSeed.Batch2Id,
                    FeedTypeId = FeedTypeSeed.StarterFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 2.4,
                    CreatedDate = new DateTime(2026, 03, 04, 7, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new FeedingLog
                {
                    Id = FeedLog2_2Id,
                    FarmingBatchId = FarmingBatchSeed.Batch2Id,
                    FeedTypeId = FeedTypeSeed.StarterFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 2.4,
                    CreatedDate = new DateTime(2026, 03, 10, 12, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new FeedingLog
                {
                    Id = FeedLog2_3Id,
                    FarmingBatchId = FarmingBatchSeed.Batch2Id,
                    FeedTypeId = FeedTypeSeed.StarterFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 2.3,
                    CreatedDate = new DateTime(2026, 03, 18, 16, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // ── Fingerling stage (Mar 22 – Apr 21) — Grower feed ───
                // Feed rate: 1.0 kg/100 fish, 5×/day. At ~740 fish ≈ 7.4 kg per feed
                new FeedingLog
                {
                    Id = FeedLog2_4Id,
                    FarmingBatchId = FarmingBatchSeed.Batch2Id,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 7.4,
                    CreatedDate = new DateTime(2026, 03, 26, 7, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new FeedingLog
                {
                    Id = FeedLog2_5Id,
                    FarmingBatchId = FarmingBatchSeed.Batch2Id,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 7.3,
                    CreatedDate = new DateTime(2026, 04, 03, 12, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new FeedingLog
                {
                    Id = FeedLog2_6Id,
                    FarmingBatchId = FarmingBatchSeed.Batch2Id,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 7.3,
                    CreatedDate = new DateTime(2026, 04, 10, 17, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new FeedingLog
                {
                    Id = FeedLog2_7Id,
                    FarmingBatchId = FarmingBatchSeed.Batch2Id,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 7.0,
                    CreatedDate = new DateTime(2026, 04, 19, 8, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // ── Juvenile stage (partial, Apr 21 – May 25) — Grower feed ──
                // Feed rate: 2.5 kg/100 fish, 3×/day. At ~700 fish ≈ 17.5 kg per feed
                new FeedingLog
                {
                    Id = FeedLog2_8Id,
                    FarmingBatchId = FarmingBatchSeed.Batch2Id,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 17.5,
                    CreatedDate = new DateTime(2026, 04, 28, 7, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new FeedingLog
                {
                    Id = FeedLog2_9Id,
                    FarmingBatchId = FarmingBatchSeed.Batch2Id,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 17.5,
                    CreatedDate = new DateTime(2026, 05, 06, 12, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new FeedingLog
                {
                    Id = FeedLog2_10Id,
                    FarmingBatchId = FarmingBatchSeed.Batch2Id,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 17.0,
                    CreatedDate = new DateTime(2026, 05, 15, 17, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new FeedingLog
                {
                    Id = FeedLog2_11Id,
                    FarmingBatchId = FarmingBatchSeed.Batch2Id,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 17.0,
                    CreatedDate = new DateTime(2026, 05, 22, 8, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
            ];
    }
}
