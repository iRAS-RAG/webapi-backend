using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class SpeciesStageConfigSeed
    {
        public static readonly Guid FryStageConfigId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000601"
        );

        public static readonly Guid FingerlingStageConfigId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000603"
        );

        public static readonly Guid JuvenileStageConfigId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000602"
        );

        public static readonly Guid GrowOutStageConfigId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000604"
        );

        // ── Crab configs ──────────────────────────────────────────────
        public static readonly Guid CrabFryStageConfigId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000605"
        );
        public static readonly Guid CrabJuvenileStageConfigId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000606"
        );
        public static readonly Guid CrabGrowOutStageConfigId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000607"
        );

        // ── Squid configs ─────────────────────────────────────────────
        public static readonly Guid SquidParalarvaStageConfigId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000608"
        );
        public static readonly Guid SquidJuvenileStageConfigId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000609"
        );
        public static readonly Guid SquidGrowOutStageConfigId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000610"
        );

        public static List<SpeciesStageConfig> SpeciesStageConfigs =>
            [
                // ── Stage 1: Fry (Cá bột) ──────────────────────────────
                new SpeciesStageConfig
                {
                    Id = FryStageConfigId,
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.FryStageId,
                    Sequence = 1,
                    AmountPer100Fish = 0.3, // 0.3 kg feed per 100 fish per round
                    FrequencyPerDay = 7, // 6–8 times/day
                    MaxStockingDensity = 300.0, // up to 300 fish/m³ in hatchery
                    ExpectedDurationDays = 21, // ~3 weeks
                    ExpectedWeightKgPerFish = 0.002, // ~2 g per fish at end
                    SurvivalRate = 0.92, // 92% survival (highest mortality in fry)
                },
                // ── Stage 2: Fingerling (Cá hương) ─────────────────────
                new SpeciesStageConfig
                {
                    Id = FingerlingStageConfigId,
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.FingerlingStageId,
                    Sequence = 2,
                    AmountPer100Fish = 1.0, // 1.0 kg feed per 100 fish per round
                    FrequencyPerDay = 5, // 4–6 times/day
                    MaxStockingDensity = 120.0, // nursery density
                    ExpectedDurationDays = 30, // ~4 weeks
                    ExpectedWeightKgPerFish = 0.02, // ~20 g per fish at end
                    SurvivalRate = 0.95, // 95% survival
                },
                // ── Stage 3: Juvenile (Cá giống) ──────────────────────
                new SpeciesStageConfig
                {
                    Id = JuvenileStageConfigId,
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.JuvenileStageId,
                    Sequence = 3,
                    AmountPer100Fish = 2.5, // 2.5 kg feed per 100 fish per round
                    FrequencyPerDay = 3, // 3–4 times/day
                    MaxStockingDensity = 40.0, // grow-out density
                    ExpectedDurationDays = 60, // ~9 weeks
                    ExpectedWeightKgPerFish = 0.15, // ~150 g per fish at end
                    SurvivalRate = 0.97, // 97% survival
                },
                // ── Stage 4: Grow-out (Cá thương phẩm) ────────────────
                new SpeciesStageConfig
                {
                    Id = GrowOutStageConfigId,
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.GrowOutStageId,
                    Sequence = 4,
                    AmountPer100Fish = 4.0, // 4.0 kg feed per 100 fish per round
                    FrequencyPerDay = 2, // 2–3 times/day
                    MaxStockingDensity = 20.0, // final grow-out density
                    ExpectedDurationDays = 90, // ~13 weeks
                    ExpectedWeightKgPerFish = 0.5, // ~500 g market size
                    SurvivalRate = 0.98, // 98% survival
                },
                // ═══════════════════════════════════════════════════════
                // Mud Crab (Cua biển)
                // ═══════════════════════════════════════════════════════

                // ── Crab Stage 1: Fry (Cua bột) ───────────────────────
                new SpeciesStageConfig
                {
                    Id = CrabFryStageConfigId,
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabFryStageId,
                    Sequence = 1,
                    AmountPer100Fish = 0.5, // 0.5 kg per 100 crab fry (8-10% body weight)
                    FrequencyPerDay = 5, // 5–6 times/day
                    MaxStockingDensity = 200.0, // up to 200 crabs/m² in nursery
                    ExpectedDurationDays = 20, // ~3 weeks
                    ExpectedWeightKgPerFish = 0.0005, // ~0.5 g at end
                    SurvivalRate = 0.80, // 80% — high mortality from cannibalism
                },
                // ── Crab Stage 2: Juvenile (Cua giống) ────────────────
                new SpeciesStageConfig
                {
                    Id = CrabJuvenileStageConfigId,
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabJuvenileStageId,
                    Sequence = 2,
                    AmountPer100Fish = 2.0, // 2.0 kg per 100 juveniles (5-7% body weight)
                    FrequencyPerDay = 4, // 3–4 times/day
                    MaxStockingDensity = 50.0, // 50/m² — more territorial
                    ExpectedDurationDays = 40, // ~6 weeks
                    ExpectedWeightKgPerFish = 0.05, // ~50 g at end
                    SurvivalRate = 0.85, // 85%
                },
                // ── Crab Stage 3: Grow-out (Cua thịt) ─────────────────
                new SpeciesStageConfig
                {
                    Id = CrabGrowOutStageConfigId,
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabGrowOutStageId,
                    Sequence = 3,
                    AmountPer100Fish = 5.0, // 5.0 kg per 100 crabs (3-5% body weight)
                    FrequencyPerDay = 2, // 2–3 times/day
                    MaxStockingDensity = 10.0, // 10/m² — highly territorial, often individual cages
                    ExpectedDurationDays = 90, // ~13 weeks
                    ExpectedWeightKgPerFish = 0.35, // ~350 g market size
                    SurvivalRate = 0.90, // 90%
                },
                // ═══════════════════════════════════════════════════════
                // Reef Squid (Mực lá)
                // ═══════════════════════════════════════════════════════

                // ── Squid Stage 1: Paralarva (Mực ấu trùng) ───────────
                new SpeciesStageConfig
                {
                    Id = SquidParalarvaStageConfigId,
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidParalarvaStageId,
                    Sequence = 1,
                    AmountPer100Fish = 0.2, // 0.2 kg per 100 paralarvae (tiny, but 8-12% body weight)
                    FrequencyPerDay = 7, // 6–8 times/day — live food continuously
                    MaxStockingDensity = 500.0, // 500/m³ in circular tanks (planktonic phase)
                    ExpectedDurationDays = 30, // ~4 weeks
                    ExpectedWeightKgPerFish = 0.0005, // ~0.5 g at end of paralarval phase
                    SurvivalRate = 0.50, // 50% — extremely high mortality in paralarval stage
                },
                // ── Squid Stage 2: Juvenile (Mực non) ─────────────────
                new SpeciesStageConfig
                {
                    Id = SquidJuvenileStageConfigId,
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidJuvenileStageId,
                    Sequence = 2,
                    AmountPer100Fish = 1.5, // 1.5 kg per 100 juveniles (5-7% body weight)
                    FrequencyPerDay = 5, // 4–5 times/day
                    MaxStockingDensity = 100.0, // 100/m³
                    ExpectedDurationDays = 60, // ~9 weeks
                    ExpectedWeightKgPerFish = 0.05, // ~50 g at end
                    SurvivalRate = 0.70, // 70% — improved after paralarval stage
                },
                // ── Squid Stage 3: Sub-adult / Grow-out (Mực thương phẩm) ──
                new SpeciesStageConfig
                {
                    Id = SquidGrowOutStageConfigId,
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidGrowOutStageId,
                    Sequence = 3,
                    AmountPer100Fish = 4.0, // 4.0 kg per 100 squid (3-5% body weight)
                    FrequencyPerDay = 3, // 2–3 times/day
                    MaxStockingDensity = 30.0, // 30/m³ — need space to avoid jetting into walls
                    ExpectedDurationDays = 90, // ~13 weeks
                    ExpectedWeightKgPerFish = 0.4, // ~400 g market size
                    SurvivalRate = 0.85, // 85%
                },
            ];
    }
}
