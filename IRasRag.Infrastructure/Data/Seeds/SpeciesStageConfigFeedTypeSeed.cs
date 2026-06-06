using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class SpeciesStageConfigFeedTypeSeed
    {
        public static List<SpeciesStageConfigFeedType> SpeciesStageConfigFeedTypes =>
            [
                // Fry — high-protein starter feed
                new SpeciesStageConfigFeedType
                {
                    SpeciesStageConfigId = SpeciesStageConfigSeed.FryStageConfigId,
                    FeedTypeId = FeedTypeSeed.StarterFeedId,
                },
                // Fingerling — grower feed
                new SpeciesStageConfigFeedType
                {
                    SpeciesStageConfigId = SpeciesStageConfigSeed.FingerlingStageConfigId,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                },
                // Juvenile — grower + finisher
                new SpeciesStageConfigFeedType
                {
                    SpeciesStageConfigId = SpeciesStageConfigSeed.JuvenileStageConfigId,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                },
                new SpeciesStageConfigFeedType
                {
                    SpeciesStageConfigId = SpeciesStageConfigSeed.JuvenileStageConfigId,
                    FeedTypeId = FeedTypeSeed.FinisherFeedId,
                },
                // Grow-out — finisher feed
                new SpeciesStageConfigFeedType
                {
                    SpeciesStageConfigId = SpeciesStageConfigSeed.GrowOutStageConfigId,
                    FeedTypeId = FeedTypeSeed.FinisherFeedId,
                },
                // ═══════════════════════════════════════════════════════
                // Mud Crab
                // ═══════════════════════════════════════════════════════

                // Crab fry — high-protein crab starter
                new SpeciesStageConfigFeedType
                {
                    SpeciesStageConfigId = SpeciesStageConfigSeed.CrabFryStageConfigId,
                    FeedTypeId = FeedTypeSeed.StarterFeedId,
                },
                // Crab juvenile — starter + grower
                new SpeciesStageConfigFeedType
                {
                    SpeciesStageConfigId = SpeciesStageConfigSeed.CrabJuvenileStageConfigId,
                    FeedTypeId = FeedTypeSeed.StarterFeedId,
                },
                new SpeciesStageConfigFeedType
                {
                    SpeciesStageConfigId = SpeciesStageConfigSeed.CrabJuvenileStageConfigId,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                },
                // Crab grow-out — crab grower feed
                new SpeciesStageConfigFeedType
                {
                    SpeciesStageConfigId = SpeciesStageConfigSeed.CrabGrowOutStageConfigId,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                },
                // ═══════════════════════════════════════════════════════
                // Reef Squid
                // ═══════════════════════════════════════════════════════

                // Squid paralarva — squid starter
                new SpeciesStageConfigFeedType
                {
                    SpeciesStageConfigId = SpeciesStageConfigSeed.SquidParalarvaStageConfigId,
                    FeedTypeId = FeedTypeSeed.StarterFeedId,
                },
                // Squid juvenile — starter + grower (transition from live to dead feed)
                new SpeciesStageConfigFeedType
                {
                    SpeciesStageConfigId = SpeciesStageConfigSeed.SquidJuvenileStageConfigId,
                    FeedTypeId = FeedTypeSeed.StarterFeedId,
                },
                new SpeciesStageConfigFeedType
                {
                    SpeciesStageConfigId = SpeciesStageConfigSeed.SquidJuvenileStageConfigId,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                },
                // Squid grow-out — squid grower
                new SpeciesStageConfigFeedType
                {
                    SpeciesStageConfigId = SpeciesStageConfigSeed.SquidGrowOutStageConfigId,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                },
            ];
    }
}
