using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Common;

namespace IRasRag.Domain.Entities
{
    public class SpeciesStageConfig : BaseEntity
    {
        [Required]
        public Guid SpeciesId { get; set; }

        [Required]
        public Guid GrowthStageId { get; set; }

        [Required]
        public Guid FeedTypeId { get; set; }

        [Required]
        public float AmountPer100Fish { get; set; } // Recommended amount in kg

        [Required]
        public int FrequencyPerDay { get; set; } // How many times to feed per day

        public float? MaxStockingDensity { get; set; } // Max fish per cubic meter (or liter) allowed in this stage

        public int? ExpectedDurationDays { get; set; } // How many days the fish typically stay in this stage

        // Navigation properties
        public Species Species { get; set; }
        public GrowthStage GrowthStage { get; set; }
        public FeedType FeedType { get; set; }
    }
}
