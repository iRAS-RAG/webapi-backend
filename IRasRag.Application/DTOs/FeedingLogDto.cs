using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    public class FeedingLogDto
    {
        public Guid Id { get; set; }
        public Guid FarmingBatchId { get; set; }
        public float Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }

    // Create DTO
    public class CreateFeedingLogDto
    {
        [Required(ErrorMessage = "Mã lô nuôi là bắt buộc")]
        public Guid FarmingBatchId { get; set; }

        [Required(ErrorMessage = "Lượng thức ăn là bắt buộc")]
        [Range(0.1, float.MaxValue, ErrorMessage = "Lượng thức ăn phải lớn hơn 0")]
        public float Amount { get; set; }

        [Required(ErrorMessage = "Ngày cho ăn là bắt buộc")]
        public DateTime CreatedDate { get; set; }
    }

    // Update DTO
    public class UpdateFeedingLogDto
    {
        public Guid? FarmingBatchId { get; set; }

        [Range(0.1, float.MaxValue, ErrorMessage = "Lượng thức ăn phải lớn hơn 0")]
        public float? Amount { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
