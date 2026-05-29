using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Common;
using IRasRag.Domain.Enums;

namespace IRasRag.Domain.Entities
{
    public class FarmingBatch : BaseEntity
    {
        [Required]
        public Guid FishTankId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public Guid CurrentStageConfigId { get; set; }

        [Required]
        [MaxLength(20)]
        public FarmingBatchStatus Status { get; set; } = FarmingBatchStatus.ACTIVE;

        [MaxLength(20)]
        public BatchPausedReason? PausedReason { get; set; } = null;

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EstimatedHarvestDate { get; set; }

        public DateTime? ActualHarvestDate { get; set; }

        [Required]
        public int InitialQuantity { get; set; }

        [Required]
        public int CurrentQuantity { get; set; }

        [Required]
        [MaxLength(20)]
        public string UnitOfMeasure { get; set; }

        // Cached estimates
        public int? EstimatedHarvestCount { get; set; }
        public double? EstimatedHarvestWeightKg { get; set; }

        // Actual harvested total weight (kg) if recorded at harvest
        public double? ActualHarvestWeightKg { get; set; }

        // Persisted Feed Conversion Ratio for this batch (kg feed / kg weight gain)
        public double? Fcr { get; set; }

        // Navigation properties
        public FishTank FishTank { get; set; }
        public SpeciesStageConfig CurrentStageConfig { get; set; }
        public ICollection<BatchStage> BatchStages { get; set; }
        public ICollection<FeedingLog> FeedingLogs { get; set; }
        public ICollection<MortalityLog> MortalityLogs { get; set; }
        public ICollection<Alert> Alerts { get; set; }
    }
}
