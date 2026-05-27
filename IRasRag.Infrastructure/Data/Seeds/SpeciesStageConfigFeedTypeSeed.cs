using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class SpeciesStageConfigFeedTypeSeed
    {
        public static List<SpeciesStageConfigFeedType> SpeciesStageConfigFeedTypes =>
            [
                new SpeciesStageConfigFeedType
                {
                    SpeciesStageConfigId = SpeciesStageConfigSeed.FryStageConfigId,
                    FeedTypeId = FeedTypeSeed.StarterFeedId,
                },
                new SpeciesStageConfigFeedType
                {
                    SpeciesStageConfigId = SpeciesStageConfigSeed.JuvenileStageConfigId,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                },
            ];
    }
}
