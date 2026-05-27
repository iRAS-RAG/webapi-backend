using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class SpeciesStageConfigSeed
    {
        public static readonly Guid FryStageConfigId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000601"
        );
        public static readonly Guid JuvenileStageConfigId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000602"
        );

        public static List<SpeciesStageConfig> SpeciesStageConfigs =>
            [
                new SpeciesStageConfig
                {
                    Id = FryStageConfigId,
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.FryStageId,
                    Sequence = 1,
                    AmountPer100Fish = 0.5, // 0.5 kg per 100 fish per feeding round
                    FrequencyPerDay = 6,
                    MaxStockingDensity = 50.0,
                    ExpectedDurationDays = 30,
                    ExpectedWeightKgPerFish = 0.01, // 10g per fish at end of fry stage
                    SurvivalRate = 0.95, // 95% expected survival through this stage
                },
                new SpeciesStageConfig
                {
                    Id = JuvenileStageConfigId,
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.JuvenileStageId,
                    Sequence = 2,
                    AmountPer100Fish = 3.0, // 3 kg per 100 fish per feeding round
                    FrequencyPerDay = 3,
                    MaxStockingDensity = 30.0,
                    ExpectedDurationDays = 90,
                    ExpectedWeightKgPerFish = 0.1, // 100g per fish at end of juvenile stage
                    SurvivalRate = 0.98, // 98% expected survival through juvenile stage
                },
            ];
    }
}
