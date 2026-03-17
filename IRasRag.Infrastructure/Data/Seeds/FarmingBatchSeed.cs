using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class FarmingBatchSeed
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

        public static readonly Guid Batch1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001501");

        public static readonly Guid Batch2Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001502");

        public static List<FarmingBatch> FarmingBatches =>
            new()
            {
                new FarmingBatch
                {
                    Id = Batch1Id,
                    FishTankId = FishTankSeed.TankAId,
                    Name = "Lô nuôi cá rô phi 2024-01",
                    CurrentStageConfigId = SpeciesStageConfigSeed.FryStageConfigId,
                    Status = FarmingBatchStatus.ACTIVE,
                    StartDate = new DateTime(2024, 01, 15, 0, 0, 0, DateTimeKind.Utc),
                    EstimatedHarvestDate = new DateTime(2024, 07, 15, 0, 0, 0, DateTimeKind.Utc),
                    ActualHarvestDate = null,
                    InitialQuantity = 1000,
                    CurrentQuantity = 950,
                    UnitOfMeasure = "con",
                    CreatedAt = SeedTimestamp,
                },
                new FarmingBatch
                {
                    Id = Batch2Id,
                    FishTankId = FishTankSeed.TankAId,
                    Name = "Lô nuôi cá rô phi 2023-12",
                    CurrentStageConfigId = SpeciesStageConfigSeed.JuvenileStageConfigId,
                    Status = FarmingBatchStatus.HARVESTED,
                    StartDate = new DateTime(2023, 12, 01, 0, 0, 0, DateTimeKind.Utc),
                    EstimatedHarvestDate = new DateTime(2024, 06, 01, 0, 0, 0, DateTimeKind.Utc),
                    ActualHarvestDate = new DateTime(2024, 05, 28, 0, 0, 0, DateTimeKind.Utc),
                    InitialQuantity = 800,
                    CurrentQuantity = 0,
                    UnitOfMeasure = "con",
                    CreatedAt = SeedTimestamp,
                },
            };
    }
}
