namespace IRasRag.Domain.Entities
{
    public class SpeciesStageConfigFeedType
    {
        public Guid SpeciesStageConfigId { get; set; }
        public Guid FeedTypeId { get; set; }

        public SpeciesStageConfig SpeciesStageConfig { get; set; }
        public FeedType FeedType { get; set; }
    }
}