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
            new()
            {
                new SpeciesStageConfig
                {
                    Id = FryStageConfigId,
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.FryStageId,
                    Sequence = 1,
                    AmountPer100Fish = 0.5,
                    FrequencyPerDay = 6,
                    MaxStockingDensity = 50.0,
                    ExpectedDurationDays = 30,
                    FeedTypes = new List<FeedType>(),
                },
                new SpeciesStageConfig
                {
                    Id = JuvenileStageConfigId,
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.JuvenileStageId,
                    Sequence = 2,
                    AmountPer100Fish = 3.0,
                    FrequencyPerDay = 3,
                    MaxStockingDensity = 30.0,
                    ExpectedDurationDays = 90,
                    FeedTypes = new List<FeedType>(),
                },
            };
    }
}
