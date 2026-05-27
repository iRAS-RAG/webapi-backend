namespace IRasRag.Application.DTOs
{
    public class RecommendedInitialDto
    {
        public Guid SpeciesId { get; set; }
        public string SpeciesName { get; set; } = string.Empty;
        public int? RecommendedInitial { get; set; }
    }
}
