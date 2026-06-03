using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class BatchStageSeed
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

        // ── Batch 1 stage IDs ──────────────────────────────────────────
        public static readonly Guid Batch1FryId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000002101"
        );
        public static readonly Guid Batch1FingerlingId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000002102"
        );
        public static readonly Guid Batch1JuvenileId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000002103"
        );
        public static readonly Guid Batch1GrowOutId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000002104"
        );

        // ── Batch 2 stage IDs ──────────────────────────────────────────
        public static readonly Guid Batch2FryId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000002201"
        );
        public static readonly Guid Batch2FingerlingId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000002202"
        );
        public static readonly Guid Batch2JuvenileId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000002203"
        );

        public static List<BatchStage> BatchStages =>
            [
                // ═══════════════════════════════════════════════════════
                // Batch 1 — Full cycle: Fry → Fingerling → Juvenile → Grow-out
                // Started 2025-08-01, harvested 2026-02-20
                // ═══════════════════════════════════════════════════════

                new BatchStage
                {
                    Id = Batch1FryId,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    SpeciesStageConfigId = SpeciesStageConfigSeed.FryStageConfigId,
                    Sequence = 1,
                    EstimatedStartDate = new DateTime(2025, 08, 01, 0, 0, 0, DateTimeKind.Utc),
                    EstimatedEndDate = new DateTime(2025, 08, 22, 0, 0, 0, DateTimeKind.Utc),
                    ExpectedDurationDays = 21,
                    ActualStartDate = new DateTime(2025, 08, 01, 0, 0, 0, DateTimeKind.Utc),
                    ActualEndDate = new DateTime(2025, 08, 22, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                    ModifiedAt = SeedTimestamp,
                },
                new BatchStage
                {
                    Id = Batch1FingerlingId,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    SpeciesStageConfigId = SpeciesStageConfigSeed.FingerlingStageConfigId,
                    Sequence = 2,
                    EstimatedStartDate = new DateTime(2025, 08, 22, 0, 0, 0, DateTimeKind.Utc),
                    EstimatedEndDate = new DateTime(2025, 09, 21, 0, 0, 0, DateTimeKind.Utc),
                    ExpectedDurationDays = 30,
                    ActualStartDate = new DateTime(2025, 08, 22, 0, 0, 0, DateTimeKind.Utc),
                    ActualEndDate = new DateTime(2025, 09, 20, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                    ModifiedAt = SeedTimestamp,
                },
                new BatchStage
                {
                    Id = Batch1JuvenileId,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    SpeciesStageConfigId = SpeciesStageConfigSeed.JuvenileStageConfigId,
                    Sequence = 3,
                    EstimatedStartDate = new DateTime(2025, 09, 21, 0, 0, 0, DateTimeKind.Utc),
                    EstimatedEndDate = new DateTime(2025, 11, 20, 0, 0, 0, DateTimeKind.Utc),
                    ExpectedDurationDays = 60,
                    ActualStartDate = new DateTime(2025, 09, 21, 0, 0, 0, DateTimeKind.Utc),
                    ActualEndDate = new DateTime(2025, 11, 21, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                    ModifiedAt = SeedTimestamp,
                },
                new BatchStage
                {
                    Id = Batch1GrowOutId,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    SpeciesStageConfigId = SpeciesStageConfigSeed.GrowOutStageConfigId,
                    Sequence = 4,
                    EstimatedStartDate = new DateTime(2025, 11, 21, 0, 0, 0, DateTimeKind.Utc),
                    EstimatedEndDate = new DateTime(2026, 02, 18, 0, 0, 0, DateTimeKind.Utc),
                    ExpectedDurationDays = 90,
                    ActualStartDate = new DateTime(2025, 11, 22, 0, 0, 0, DateTimeKind.Utc),
                    ActualEndDate = new DateTime(2026, 02, 20, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                    ModifiedAt = SeedTimestamp,
                },
                // ═══════════════════════════════════════════════════════
                // Batch 2 — Early harvest: Fry → Fingerling → Juvenile (partial)
                // Started 2026-03-01, harvested 2026-05-25
                // ═══════════════════════════════════════════════════════

                new BatchStage
                {
                    Id = Batch2FryId,
                    FarmingBatchId = FarmingBatchSeed.Batch2Id,
                    SpeciesStageConfigId = SpeciesStageConfigSeed.FryStageConfigId,
                    Sequence = 1,
                    EstimatedStartDate = new DateTime(2026, 03, 01, 0, 0, 0, DateTimeKind.Utc),
                    EstimatedEndDate = new DateTime(2026, 03, 22, 0, 0, 0, DateTimeKind.Utc),
                    ExpectedDurationDays = 21,
                    ActualStartDate = new DateTime(2026, 03, 01, 0, 0, 0, DateTimeKind.Utc),
                    ActualEndDate = new DateTime(2026, 03, 22, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                    ModifiedAt = SeedTimestamp,
                },
                new BatchStage
                {
                    Id = Batch2FingerlingId,
                    FarmingBatchId = FarmingBatchSeed.Batch2Id,
                    SpeciesStageConfigId = SpeciesStageConfigSeed.FingerlingStageConfigId,
                    Sequence = 2,
                    EstimatedStartDate = new DateTime(2026, 03, 22, 0, 0, 0, DateTimeKind.Utc),
                    EstimatedEndDate = new DateTime(2026, 04, 21, 0, 0, 0, DateTimeKind.Utc),
                    ExpectedDurationDays = 30,
                    ActualStartDate = new DateTime(2026, 03, 22, 0, 0, 0, DateTimeKind.Utc),
                    ActualEndDate = new DateTime(2026, 04, 21, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                    ModifiedAt = SeedTimestamp,
                },
                new BatchStage
                {
                    Id = Batch2JuvenileId,
                    FarmingBatchId = FarmingBatchSeed.Batch2Id,
                    SpeciesStageConfigId = SpeciesStageConfigSeed.JuvenileStageConfigId,
                    Sequence = 3,
                    EstimatedStartDate = new DateTime(2026, 04, 21, 0, 0, 0, DateTimeKind.Utc),
                    EstimatedEndDate = new DateTime(2026, 06, 20, 0, 0, 0, DateTimeKind.Utc),
                    ExpectedDurationDays = 60,
                    ActualStartDate = new DateTime(2026, 04, 21, 0, 0, 0, DateTimeKind.Utc),
                    ActualEndDate = new DateTime(2026, 05, 25, 0, 0, 0, DateTimeKind.Utc), // early harvest at 34 days
                    CreatedAt = SeedTimestamp,
                    ModifiedAt = SeedTimestamp,
                },
            ];
    }
}
