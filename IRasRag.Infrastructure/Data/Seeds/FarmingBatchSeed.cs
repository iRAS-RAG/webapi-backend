using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class FarmingBatchSeed
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

        public static readonly Guid Batch1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001501");

        public static readonly Guid Batch2Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001502");

        public static List<FarmingBatch> FarmingBatches =>
            [
                new FarmingBatch
                {
                    Id = Batch1Id,
                    FishTankId = FishTankSeed.TankAId,
                    Name = "Vụ nuôi cá rô phi 2026-05",
                    CurrentStageConfigId = SpeciesStageConfigSeed.FryStageConfigId,
                    Status = FarmingBatchStatus.ACTIVE,
                    StartDate = new DateTime(2026, 05, 15, 0, 0, 0, DateTimeKind.Utc),
                    EstimatedHarvestDate = new DateTime(2026, 07, 15, 0, 0, 0, DateTimeKind.Utc),
                    ActualHarvestDate = null,
                    InitialQuantity = 1000,
                    CurrentQuantity = 950,
                    UnitOfMeasure = "con",
                    EstimatedHarvestCount = 900,
                    EstimatedHarvestWeightKg = 90.0,
                    CreatedAt = SeedTimestamp,
                    ModifiedAt = SeedTimestamp,
                },
                new FarmingBatch
                {
                    Id = Batch2Id,
                    FishTankId = FishTankSeed.TankAId,
                    Name = "Vụ nuôi cá rô phi 2026-01",
                    CurrentStageConfigId = SpeciesStageConfigSeed.JuvenileStageConfigId,
                    Status = FarmingBatchStatus.HARVESTED,
                    StartDate = new DateTime(2026, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                    EstimatedHarvestDate = new DateTime(2026, 04, 01, 0, 0, 0, DateTimeKind.Utc),
                    ActualHarvestDate = new DateTime(2026, 04, 02, 0, 0, 0, DateTimeKind.Utc),
                    InitialQuantity = 800,
                    CurrentQuantity = 790,
                    UnitOfMeasure = "con",
                    EstimatedHarvestCount = 780,
                    EstimatedHarvestWeightKg = 78.0,
                    CreatedAt = SeedTimestamp,
                    ModifiedAt = SeedTimestamp,
                },
            ];
    }
}
