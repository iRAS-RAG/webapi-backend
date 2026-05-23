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
        public double AmountPer100Fish { get; set; } // Recommended amount in kg

        [Required]
        public int FrequencyPerDay { get; set; } // How many times to feed per day

        public double? MaxStockingDensity { get; set; } // Max fish per cubic meter (or liter) allowed in this stage

        public int? ExpectedDurationDays { get; set; } // How many days the fish typically stay in this stage

        // Sequence defines the order of this stage within the species' stage list (1-based)
        [Required]
        public int Sequence { get; set; }

        // Navigation properties
        public Species Species { get; set; }
        public GrowthStage GrowthStage { get; set; }
        public ICollection<FeedType> FeedTypes { get; set; }
        public ICollection<SpeciesStageConfigFeedType> SpeciesStageConfigFeedTypes { get; set; }
        public ICollection<FarmingBatch> FarmingBatches { get; set; }
        public ICollection<BatchStage> BatchStages { get; set; }
    }
}
