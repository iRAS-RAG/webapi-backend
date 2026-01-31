using IRasRag.Domain.Common;
using IRasRag.Domain.Enums;
using System.ComponentModel.DataAnnotations;

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
        public Guid SpeciesId { get; set; }

        [Required]
        [MaxLength(20)]
        public FarmingBatchStatus Status { get; set; } = FarmingBatchStatus.ACTIVE;

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EstimatedHarvestDate { get; set; }

        public DateTime? ActualHarvestDate { get; set; }

        [Required]
        public float InitialQuantity { get; set; }

        [Required]
        public float CurrentQuantity { get; set; }

        [Required]
        [MaxLength(20)]
        public string UnitOfMeasure { get; set; }

        // Navigation properties
        public FishTank FishTank { get; set; }
        public Species Species { get; set; }
        public ICollection<FeedingLog> FeedingLogs { get; set; }
        public ICollection<MortalityLog> MortalityLogs { get; set; }
        public ICollection<Alert> Alerts { get; set; }
    }
}
