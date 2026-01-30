using IRasRag.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace IRasRag.Domain.Entities
{
    public class MasterBoard : BaseEntity
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

        // Navigation properties
        public FishTank FishTank { get; set; }
        public ICollection<Sensor> Sensors { get; set; }
        public ICollection<ControlDevice> ControlDevices { get; set; }
    }
}
