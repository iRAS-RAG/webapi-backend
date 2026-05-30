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

        // Create a couple of realistic batch stages for the seeded batches
        public static readonly Guid Batch1Stage1Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000002101"
        );
        public static readonly Guid Batch1Stage2Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000002102"
        );
        public static readonly Guid Batch2Stage1Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000002201"
        );

        public static List<BatchStage> BatchStages =>
            [
                // Batch 1 - currently in Fry stage
                new BatchStage
                {
                    Id = Batch1Stage1Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    SpeciesStageConfigId = SpeciesStageConfigSeed.FryStageConfigId,
                    Sequence = 1,
                    EstimatedStartDate = new DateTime(2024, 01, 15, 0, 0, 0, DateTimeKind.Utc),
                    EstimatedEndDate = new DateTime(2024, 02, 14, 0, 0, 0, DateTimeKind.Utc),
                    ExpectedDurationDays = 30,
                    ActualStartDate = new DateTime(2024, 01, 15, 0, 0, 0, DateTimeKind.Utc),
                    ActualEndDate = null,
                    CreatedAt = SeedTimestamp,
                    ModifiedAt = SeedTimestamp,
                },
                // Batch 1 - upcoming Juvenile stage (not started yet)
                new BatchStage
                {
                    Id = Batch1Stage2Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    SpeciesStageConfigId = SpeciesStageConfigSeed.JuvenileStageConfigId,
                    Sequence = 2,
                    EstimatedStartDate = new DateTime(2024, 02, 15, 0, 0, 0, DateTimeKind.Utc),
                    EstimatedEndDate = new DateTime(2024, 05, 15, 0, 0, 0, DateTimeKind.Utc),
                    ExpectedDurationDays = 90,
                    ActualStartDate = null,
                    ActualEndDate = null,
                    CreatedAt = SeedTimestamp,
                    ModifiedAt = SeedTimestamp,
                },
                // Batch 2 - already completed Fry stage
                new BatchStage
                {
                    Id = Batch2Stage1Id,
                    FarmingBatchId = FarmingBatchSeed.Batch2Id,
                    SpeciesStageConfigId = SpeciesStageConfigSeed.FryStageConfigId,
                    Sequence = 1,
                    EstimatedStartDate = new DateTime(2026, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                    EstimatedEndDate = new DateTime(2026, 04, 01, 0, 0, 0, DateTimeKind.Utc),
                    ExpectedDurationDays = 30,
                    ActualStartDate = new DateTime(2026, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                    ActualEndDate = new DateTime(2026, 04, 02, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                    ModifiedAt = SeedTimestamp,
                },
            ];
    }
}
