using IRasRag.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace IRasRag.Domain.Entities
{
    public class Sensor : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public int PinCode { get; set; }

        [Required]
        public Guid SensorTypeId { get; set; }

        [Required]
        public Guid MasterBoardId { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public SensorType SensorType { get; set; }
        public MasterBoard MasterBoard { get; set; }
        public ICollection<SensorLog> SensorLogs { get; set; }
        public ICollection<Job> Jobs { get; set; }
    }
}
