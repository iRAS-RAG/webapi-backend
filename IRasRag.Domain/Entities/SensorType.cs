using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Common;

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

        [MaxLength(50)]
        public string Code { get; set; }

        /// <summary>
        /// The minimum physically/chemically possible value for this sensor type.
        /// Used to validate safe thresholds set by Supervisors.
        /// </summary>
        public double MinPossibleValue { get; set; }

        /// <summary>
        /// The maximum physically/chemically possible value for this sensor type.
        /// Used to validate safe thresholds set by Supervisors.
        /// </summary>
        public double MaxPossibleValue { get; set; }

        // Navigation properties
        public ICollection<Sensor> Sensors { get; set; }
        public ICollection<SpeciesThreshold> SpeciesThresholds { get; set; }
        public ICollection<Alert> Alerts { get; set; }
    }
}
