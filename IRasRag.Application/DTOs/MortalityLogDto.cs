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
        public double LostWeightKg { get; set; }
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

        [Required(ErrorMessage = "Tổng trọng lượng hao hụt (kg) là bắt buộc")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "Tổng trọng lượng phải lớn hơn 0")]
        public double LostWeightKg { get; set; }

        [Required(ErrorMessage = "Ngày ghi nhận là bắt buộc")]
        public DateTime Date { get; set; }
    }

    // Validation result DTO for mortality estimates
    public class MortalityValidationResultDto
    {
        // Estimated average kg per fish used for the calculation (nullable if unknown)
        public double? PerFishKg { get; set; }

        // Expected total lost kg = PerFishKg * Quantity (nullable if PerFishKg unknown)
        public double? ExpectedLostKg { get; set; }

        // Allowed range for total lost kg (nullable if ExpectedLostKg unknown)
        public double? MinAllowedKg { get; set; }
        public double? MaxAllowedKg { get; set; }

        // Indicates whether the provided lost weight (if supplied by client) is within the allowed range
        public bool? IsWithinRange { get; set; }

        // Human-friendly advisory message
        public string Message { get; set; } = string.Empty;
    }

    // Update DTO
    public class UpdateMortalityLogDto
    {
        public Guid? BatchId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int? Quantity { get; set; }

        [Range(0.0001, double.MaxValue, ErrorMessage = "Tổng trọng lượng phải lớn hơn 0")]
        public double? LostWeightKg { get; set; }

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

        // Nullable for validation (weight unknown yet); required when actually logging.
        public double? LostWeightKg { get; set; }

        [Required(ErrorMessage = "Ngày ghi nhận là bắt buộc")]
        public DateTime Date { get; set; }
    }
}
