using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Common;

namespace IRasRag.Domain.Entities
{
    public class ControlDevice : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public int PinCode { get; set; }

        [Required]
        public bool State { get; set; }

        [Required]
        [MaxLength(50)]
        public string CommandOn { get; set; }

        [Required]
        [MaxLength(50)]
        public string CommandOff { get; set; }

        [Required]
        public Guid MasterBoardId { get; set; }

        [Required]
        public Guid ControlDeviceTypeId { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public MasterBoard MasterBoard { get; set; }
        public ControlDeviceType ControlDeviceType { get; set; }
        public ICollection<JobControlMapping> JobControlMappings { get; set; }
    }
}
