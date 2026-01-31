using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Common;
using IRasRag.Domain.Enums;

namespace IRasRag.Domain.Entities
{
    public class JobControlMapping : BaseEntity
    {
        [Required]
        public Guid JobId { get; set; }

        [Required]
        public Guid ControlDeviceId { get; set; }

        [Required]
        public bool TargetState { get; set; }

        [Required]
        [MaxLength(50)]
        public JobTriggerCondition TriggerCondition { get; set; } = JobTriggerCondition.ALWAYS;

        // Navigation properties
        public Job Job { get; set; }
        public ControlDevice ControlDevice { get; set; }
    }
}
