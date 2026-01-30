using IRasRag.Domain.Common;
using IRasRag.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace IRasRag.Domain.Entities
{
    public class Alert : BaseEntity
    {
        [Required]
        public Guid SensorLogId { get; set; }

        [Required]
        public Guid SpeciesThresholdId { get; set; }

        public Guid? FarmingBatchId { get; set; }

        [Required]
        public Guid FishTankId { get; set; }

        [Required]
        public Guid SensorTypeId { get; set; }

        [Required]
        public float Value { get; set; }

        [Required]
        public DateTime RaisedAt { get; set; }

        public DateTime? ResolvedAt { get; set; }

        [Required]
        [MaxLength(20)]
        public AlertStatus Status { get; set; } = AlertStatus.OPEN;

        // Navigation properties
        public SensorLog SensorLog { get; set; }
        public SpeciesThreshold SpeciesThreshold { get; set; }
        public FarmingBatch? FarmingBatch { get; set; }
        public FishTank FishTank { get; set; }
        public SensorType SensorType { get; set; }
        public ICollection<CorrectiveAction> CorrectiveActions { get; set; }
        public ICollection<Recommendation> Recommendations { get; set; }
    }
}
