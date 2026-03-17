using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Common;

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
        public double ProteinPercentage { get; set; } // % of protein

        [MaxLength(255)]
        public string Manufacturer { get; set; }

        // Navigation properties
        public ICollection<SpeciesStageConfig> SpeciesStageConfigs { get; set; }
    }
}
