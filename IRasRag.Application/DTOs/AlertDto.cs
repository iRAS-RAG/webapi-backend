using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Enums;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    public class AlertDto
    {
        public Guid Id { get; set; }
        public Guid SensorLogId { get; set; }
        public Guid SpeciesThresholdId { get; set; }
        public Guid? FarmingBatchId { get; set; }
        public string? FarmingBatchName { get; set; }
        public Guid FishTankId { get; set; }
        public string FishTankName { get; set; } = string.Empty;
        public Guid SensorTypeId { get; set; }
        public string SensorTypeName { get; set; } = string.Empty;
        public float Value { get; set; }
        public DateTime RaisedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public AlertStatus Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }

    // Create DTO
    public class CreateAlertDto
    {
        [Required(ErrorMessage = "SensorLogId là bắt buộc")]
        public Guid SensorLogId { get; set; }

        [Required(ErrorMessage = "SpeciesThresholdId là bắt buộc")]
        public Guid SpeciesThresholdId { get; set; }

        public Guid? FarmingBatchId { get; set; }

        [Required(ErrorMessage = "FishTankId là bắt buộc")]
        public Guid FishTankId { get; set; }

        [Required(ErrorMessage = "SensorTypeId là bắt buộc")]
        public Guid SensorTypeId { get; set; }

        [Required(ErrorMessage = "Giá trị là bắt buộc")]
        public float Value { get; set; }

        [Required(ErrorMessage = "Thời gian phát hiện là bắt buộc")]
        public DateTime RaisedAt { get; set; }
    }

    // Update DTO
    public class UpdateAlertDto
    {
        public Guid? SensorLogId { get; set; }

        public Guid? SpeciesThresholdId { get; set; }

        public Guid? FarmingBatchId { get; set; }

        public Guid? FishTankId { get; set; }

        public Guid? SensorTypeId { get; set; }

        public float? Value { get; set; }

        public DateTime? RaisedAt { get; set; }

        public DateTime? ResolvedAt { get; set; }

        public AlertStatus? Status { get; set; }
    }
}
