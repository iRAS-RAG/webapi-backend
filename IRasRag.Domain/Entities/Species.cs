using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Common;

namespace IRasRag.Domain.Entities
{
    public class Species : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        // Navigation properties
        public ICollection<FarmingBatch> FarmingBatches { get; set; }
        public ICollection<SpeciesThreshold> SpeciesThresholds { get; set; }
        public ICollection<SpeciesStageConfig> SpeciesStageConfigs { get; set; }
    }
}
