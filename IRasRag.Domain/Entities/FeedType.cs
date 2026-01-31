using IRasRag.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace IRasRag.Domain.Entities
{
    public class FeedType : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public float WeightPerUnit { get; set; } // e.g., weight of one bag or pellet in kg

        [Required]
        public float ProteinPercentage { get; set; } // % of protein

        [MaxLength(255)]
        public string Manufacturer { get; set; }

        // Navigation properties
        public ICollection<SpeciesStageConfig> SpeciesStageConfigs { get; set; }
    }
}
