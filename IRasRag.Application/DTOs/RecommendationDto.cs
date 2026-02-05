using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    public class RecommendationDto
    {
        public Guid Id { get; set; }
        public Guid AlertId { get; set; }
        public Guid DocumentId { get; set; }
        public string DocumentTitle { get; set; } = string.Empty;
        public string SuggestionText { get; set; } = string.Empty;
    }

    // Create DTO
    public class CreateRecommendationDto
    {
        [Required(ErrorMessage = "Id cảnh báo là bắt buộc")]
        public Guid AlertId { get; set; }

        [Required(ErrorMessage = "Id tài liệu là bắt buộc")]
        public Guid DocumentId { get; set; }

        [Required(ErrorMessage = "Nội dung khuyến nghị là bắt buộc")]
        public string SuggestionText { get; set; } = string.Empty;
    }

    // Update DTO
    public class UpdateRecommendationDto
    {
        public Guid? AlertId { get; set; }

        public Guid? DocumentId { get; set; }

        public string? SuggestionText { get; set; }
    }
}
