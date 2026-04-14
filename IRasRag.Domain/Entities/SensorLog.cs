using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Common;

namespace IRasRag.Domain.Entities
{
    public class SensorLog : BaseEntity
    {
        [Required]
        public Guid SensorId { get; set; }

        [Required]
        public DateTime PeriodStart { get; set; }

        [Required]
        public double Average { get; set; }

        [Required]
        public double Min { get; set; }

        [Required]
        public double Max { get; set; }

        [Required]
        public int SampleCount { get; set; }

        [Required]
        public bool HasWarning { get; set; }

        // Navigation properties
        public Sensor Sensor { get; set; }
    }
}
