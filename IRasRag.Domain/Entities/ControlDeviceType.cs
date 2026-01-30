using IRasRag.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace IRasRag.Domain.Entities
{
    public class ControlDeviceType : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public string Description { get; set; }

        // Navigation properties
        public ICollection<ControlDevice> ControlDevices { get; set; }
    }
}
