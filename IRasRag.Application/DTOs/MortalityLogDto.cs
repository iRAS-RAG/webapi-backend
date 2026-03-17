using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    public class MortalityLogDto
    {
        public Guid Id { get; set; }
        public Guid BatchId { get; set; }
        public string BatchName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }

    // Create DTO
    public class CreateMortalityLogDto
    {
        [Required(ErrorMessage = "Mã lô nuôi là bắt buộc")]
        public Guid BatchId { get; set; }

        [JsonIgnore]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Ngày ghi nhận là bắt buộc")]
        public DateTime Date { get; set; }
    }

    // Update DTO
    public class UpdateMortalityLogDto
    {
        public Guid? BatchId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int? Quantity { get; set; }

        public DateTime? Date { get; set; }
    }

    // List Request DTO
    public class MortalityLogListRequest : BasePaginatedListRequest
    {
        public Guid? BatchId { get; set; }
    }

    // Batch sub-route request
    public class LogMortalityRequest
    {
        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Ngày ghi nhận là bắt buộc")]
        public DateTime Date { get; set; }
    }
}
