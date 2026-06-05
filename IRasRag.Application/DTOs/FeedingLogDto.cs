using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    public class FeedingLogDto
    {
        public Guid Id { get; set; }
        public Guid FarmingBatchId { get; set; }
        public string FarmingBatchName { get; set; } = string.Empty;
        public Guid FeedTypeId { get; set; }
        public string FeedTypeName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public double Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }

    // Create DTO
    public class CreateFeedingLogDto
    {
        [JsonIgnore]
        public Guid FarmingBatchId { get; set; }

        [Required(ErrorMessage = "Loại thức ăn là bắt buộc")]
        public Guid FeedTypeId { get; set; }

        [JsonIgnore]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Lượng thức ăn là bắt buộc")]
        [Range(0.0, double.MaxValue, ErrorMessage = "Lượng thức ăn phải lớn hơn 0")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "Ngày cho ăn là bắt buộc")]
        public DateTime CreatedDate { get; set; }
    }

    // Update DTO
    public class UpdateFeedingLogDto
    {
        public Guid? FarmingBatchId { get; set; }

        public Guid? FeedTypeId { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "Lượng thức ăn phải lớn hơn 0")]
        public double? Amount { get; set; }

        public DateTime? CreatedDate { get; set; }
    }

    // List Request DTO
    public class FeedingLogListRequest : BasePaginatedListRequest
    {
        public DateTime? CreatedDate { get; set; }

        [JsonIgnore]
        public Guid? FarmingBatchId { get; set; }
        public Guid? FeedTypeId { get; set; }
    }

    // Batch sub-route requests
    public class RecordFeedingRequest
    {
        [Required(ErrorMessage = "Lượng thức ăn là bắt buộc")]
        [Range(0.0, double.MaxValue, ErrorMessage = "Lượng thức ăn phải lớn hơn 0")]
        public double Amount { get; set; }
    }
}
