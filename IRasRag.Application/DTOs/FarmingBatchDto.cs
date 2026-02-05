using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Enums;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    public class FarmingBatchDto
    {
        public Guid Id { get; set; }
        public Guid FishTankId { get; set; }
        public string FishTankName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Guid SpeciesId { get; set; }
        public string SpeciesName { get; set; } = string.Empty;
        public FarmingBatchStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EstimatedHarvestDate { get; set; }
        public DateTime? ActualHarvestDate { get; set; }
        public float InitialQuantity { get; set; }
        public float CurrentQuantity { get; set; }
        public string UnitOfMeasure { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }

    // Create DTO
    public class CreateFarmingBatchDto
    {
        [Required(ErrorMessage = "FishTankId là bắt buộc")]
        public Guid FishTankId { get; set; }

        [Required(ErrorMessage = "Tên lô nuôi là bắt buộc")]
        [MaxLength(255, ErrorMessage = "Tên lô nuôi không được vượt quá 255 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "SpeciesId là bắt buộc")]
        public Guid SpeciesId { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateTime StartDate { get; set; }

        public DateTime? EstimatedHarvestDate { get; set; }

        [Required(ErrorMessage = "Số lượng ban đầu là bắt buộc")]
        [Range(0, float.MaxValue, ErrorMessage = "Số lượng ban đầu phải lớn hơn hoặc bằng 0")]
        public float InitialQuantity { get; set; }

        [Required(ErrorMessage = "Đơn vị đo là bắt buộc")]
        [MaxLength(20, ErrorMessage = "Đơn vị đo không được vượt quá 20 ký tự")]
        public string UnitOfMeasure { get; set; } = string.Empty;
    }

    // Update DTO
    public class UpdateFarmingBatchDto
    {
        [MaxLength(255, ErrorMessage = "Tên lô nuôi không được vượt quá 255 ký tự")]
        public string? Name { get; set; }

        public FarmingBatchStatus? Status { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EstimatedHarvestDate { get; set; }

        public DateTime? ActualHarvestDate { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "Số lượng hiện tại phải lớn hơn hoặc bằng 0")]
        public float? CurrentQuantity { get; set; }

        [MaxLength(20, ErrorMessage = "Đơn vị đo không được vượt quá 20 ký tự")]
        public string? UnitOfMeasure { get; set; }
    }
}
