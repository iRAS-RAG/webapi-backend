using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Common;

namespace IRasRag.Domain.Entities
{
    public class MasterBoard : BaseEntity, ISoftDeletable
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string MacAddress { get; set; }

        [Required]
        public Guid FishTankId { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public FishTank FishTank { get; set; }
        public ICollection<Sensor> Sensors { get; set; }
        public ICollection<ControlDevice> ControlDevices { get; set; }
    }
}
