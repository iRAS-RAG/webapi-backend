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
                // ── Batch 1: Full grow-out cycle, completed ──────────────
                // 2025-08-01 → 2026-02-20 (203 days, 2 days over estimate)
                new FarmingBatch
                {
                    Id = Batch1Id,
                    FishTankId = FishTankSeed.TankAId,
                    Name = "Vụ nuôi cá rô phi 2025-08",
                    CurrentStageConfigId = SpeciesStageConfigSeed.GrowOutStageConfigId,
                    Status = FarmingBatchStatus.HARVESTED,
                    StartDate = new DateTime(2025, 08, 01, 0, 0, 0, DateTimeKind.Utc),
                    EstimatedHarvestDate = new DateTime(2026, 02, 18, 0, 0, 0, DateTimeKind.Utc),
                    ActualHarvestDate = new DateTime(2026, 02, 20, 0, 0, 0, DateTimeKind.Utc),
                    InitialQuantity = 1000,
                    CurrentQuantity = 825, // harvested count
                    UnitOfMeasure = "con",
                    EstimatedHarvestCount = 830, // projected ~83% cumulative survival
                    EstimatedHarvestWeightKg = 415.0,
                    ActualHarvestWeightKg = 412.5, // 825 × 0.5 kg
                    Fcr = 1.55, // typical full-cycle tilapia FCR
                    CreatedAt = SeedTimestamp,
                    ModifiedAt = SeedTimestamp,
                },
                // ── Batch 2: Early harvest at juvenile size ──────────────
                // 2026-03-01 → 2026-05-25 (85 days, sold early for market)
                new FarmingBatch
                {
                    Id = Batch2Id,
                    FishTankId = FishTankSeed.TankAId,
                    Name = "Vụ nuôi cá rô phi 2026-03",
                    CurrentStageConfigId = SpeciesStageConfigSeed.JuvenileStageConfigId,
                    Status = FarmingBatchStatus.HARVESTED,
                    StartDate = new DateTime(2026, 03, 01, 0, 0, 0, DateTimeKind.Utc),
                    EstimatedHarvestDate = new DateTime(2026, 06, 20, 0, 0, 0, DateTimeKind.Utc),
                    ActualHarvestDate = new DateTime(2026, 05, 25, 0, 0, 0, DateTimeKind.Utc),
                    InitialQuantity = 800,
                    CurrentQuantity = 700, // harvested count
                    UnitOfMeasure = "con",
                    EstimatedHarvestCount = 680, // projected
                    EstimatedHarvestWeightKg = 70.0, // 700 × ~0.10 kg (early juvenile)
                    ActualHarvestWeightKg = 73.5, // actual weighed at harvest
                    Fcr = 1.30, // early harvest — more efficient conversion
                    CreatedAt = SeedTimestamp,
                    ModifiedAt = SeedTimestamp,
                },
            ];
    }
}
