using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Common;

namespace IRasRag.Domain.Entities
{
    public class SpeciesThreshold : BaseEntity
    {
        [Required]
        public Guid SpeciesId { get; set; }

        [Required]
        public Guid GrowthStageId { get; set; }

        [Required]
        public Guid SensorTypeId { get; set; }

        [Required]
        public float MinValue { get; set; }

        [Required]
        public float MaxValue { get; set; }

        // Navigation properties
        public Species Species { get; set; }
        public GrowthStage GrowthStage { get; set; }
        public SensorType SensorType { get; set; }
        public ICollection<Alert> Alerts { get; set; }
    }
}
