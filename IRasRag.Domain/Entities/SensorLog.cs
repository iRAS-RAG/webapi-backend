using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IRasRag.Domain.Common;

namespace IRasRag.Domain.Entities
{
    public class SensorLog : BaseEntity
    {
        [Required]
        public Guid SensorId { get; set; }

        [Required]
        public double Data { get; set; }

        [Required]
        public bool IsWarning { get; set; }

        [Required]
        public string DataJson { get; set; } = "{}";

        // Navigation properties
        public Sensor Sensor { get; set; }
        public ICollection<Alert> Alerts { get; set; }
    }
}
