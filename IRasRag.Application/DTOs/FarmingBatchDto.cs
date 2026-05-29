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
        public double TankVolume { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid SpeciesStageConfigId { get; set; }
        public Guid SpeciesId { get; set; }
        public string SpeciesName { get; set; } = string.Empty;

        // Planned stages for the batch
        public IReadOnlyList<PlannedStageDto> PlannedStages { get; set; } =
            new List<PlannedStageDto>();
        public string StageName { get; set; } = string.Empty;
        public FarmingBatchStatus Status { get; set; }
        public BatchPausedReason? PausedReason { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EstimatedHarvestDate { get; set; }
        public DateTime? ActualHarvestDate { get; set; }
        public int InitialQuantity { get; set; }
        public int CurrentQuantity { get; set; }
        public string UnitOfMeasure { get; set; } = string.Empty;
        public int? EstimatedHarvestCount { get; set; }
        public double? EstimatedHarvestWeightKg { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }

    public class ActiveFarmingBatchResponseDto
    {
        public string FarmingBatchName { get; set; } = string.Empty;
        public string FishTankName { get; set; } = string.Empty;
        public string SpeciesName { get; set; } = string.Empty;
        public int CurrentQuantity { get; set; }
        public double TankVolume { get; set; }
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

        [Required(ErrorMessage = "Số lượng ban đầu là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng ban đầu phải lớn hơn hoặc bằng 0")]
        public int InitialQuantity { get; set; }

        [Required(ErrorMessage = "Đơn vị đo là bắt buộc")]
        [MaxLength(20, ErrorMessage = "Đơn vị đo không được vượt quá 20 ký tự")]
        public string UnitOfMeasure { get; set; } = string.Empty;
    }

    public class PlannedStageDto
    {
        public Guid Id { get; set; }
        public int Sequence { get; set; }
        public Guid SpeciesStageConfigId { get; set; }
        public Guid GrowthStageId { get; set; }
        public string StageName { get; set; } = string.Empty;

        // Stage duration is intentionally omitted to avoid duplicating species-stage-config data
        public DateTime EstimatedStartDate { get; set; }
        public DateTime EstimatedEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }

        // Minimal config values kept for stage context
        public int FrequencyPerDay { get; set; }
        public IReadOnlyList<string> FeedTypeNames { get; set; } = new List<string>();

        // Calculated fields specific to this batch and stage
        public int ExpectedCount { get; set; }
        public double ExpectedTotalWeightKg { get; set; }
        public double EstimatedDailyFeedKg { get; set; }

        // Minimal config growth fields
        public double? ExpectedWeightKgPerFish { get; set; }
    }

    // Update DTO
    public class UpdateFarmingBatchDto
    {
        [MaxLength(255, ErrorMessage = "Tên lô nuôi không được vượt quá 255 ký tự")]
        public string? Name { get; set; }

        public FarmingBatchStatus? Status { get; set; }

        public BatchPausedReason? PausedReason { get; set; }

        public Guid? SpeciesStageConfigId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EstimatedHarvestDate { get; set; }

        public DateTime? ActualHarvestDate { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng hiện tại phải lớn hơn hoặc bằng 0")]
        public int? CurrentQuantity { get; set; }

        [MaxLength(20, ErrorMessage = "Đơn vị đo không được vượt quá 20 ký tự")]
        public string? UnitOfMeasure { get; set; }
    }

    // List Request DTO
    public class FarmingBatchListRequest : BasePaginatedListRequest
    {
        public FarmingBatchStatus? Status { get; set; }
        public Guid? FishTankId { get; set; }
    }
}
