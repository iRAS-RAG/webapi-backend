using IRasRag.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace IRasRag.Domain.Entities
{
    public class SensorType : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string MeasureType { get; set; }

        [Required]
        [MaxLength(50)]
        public string UnitOfMeasure { get; set; }

        // Navigation properties
        public ICollection<Sensor> Sensors { get; set; }
        public ICollection<SpeciesThreshold> SpeciesThresholds { get; set; }
        public ICollection<Alert> Alerts { get; set; }
    }
}
