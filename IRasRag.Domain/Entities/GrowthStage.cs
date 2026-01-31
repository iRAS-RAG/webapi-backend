using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Common;

namespace IRasRag.Domain.Entities
{
    public class GrowthStage : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        // Navigation properties
        public ICollection<SpeciesThreshold> SpeciesThresholds { get; set; }
        public ICollection<SpeciesStageConfig> SpeciesStageConfigs { get; set; }
    }
}
