using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Common;

namespace IRasRag.Domain.Entities
{
    public class BatchStage : BaseEntity
    {
        [Required]
        public Guid FarmingBatchId { get; set; }

        [Required]
        public Guid SpeciesStageConfigId { get; set; }

        [Required]
        public int Sequence { get; set; }

        [Required]
        public DateTime EstimatedStartDate { get; set; }

        [Required]
        public DateTime EstimatedEndDate { get; set; }

        [Required]
        public int ExpectedDurationDays { get; set; }

        public DateTime? ActualStartDate { get; set; }

        public DateTime? ActualEndDate { get; set; }

        // Navigation
        public FarmingBatch FarmingBatch { get; set; }
        public SpeciesStageConfig SpeciesStageConfig { get; set; }
    }
}
