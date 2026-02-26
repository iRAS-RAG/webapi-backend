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
                    FeedTypeId = FeedTypeSeed.StarterFeedId,
                    AmountPer100Fish = 0.5f,
                    FrequencyPerDay = 6,
                    MaxStockingDensity = 50.0f,
                    ExpectedDurationDays = 30,
                },
                new SpeciesStageConfig
                {
                    Id = JuvenileStageConfigId,
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.JuvenileStageId,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                    AmountPer100Fish = 3.0f,
                    FrequencyPerDay = 3,
                    MaxStockingDensity = 30.0f,
                    ExpectedDurationDays = 90,
                },
            };
    }
}
