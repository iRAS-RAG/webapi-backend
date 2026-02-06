using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Enums;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    public class JobControlMappingDto
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        public string JobName { get; set; } = string.Empty;
        public Guid ControlDeviceId { get; set; }
        public string ControlDeviceName { get; set; } = string.Empty;
        public bool TargetState { get; set; }
        public JobTriggerCondition TriggerCondition { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }

    // Create DTO
    public class CreateJobControlMappingDto
    {
        [Required(ErrorMessage = "JobId là bắt buộc")]
        public Guid JobId { get; set; }

        [Required(ErrorMessage = "ControlDeviceId là bắt buộc")]
        public Guid ControlDeviceId { get; set; }

        [Required(ErrorMessage = "TargetState là bắt buộc")]
        public bool TargetState { get; set; }

        [Required(ErrorMessage = "TriggerCondition là bắt buộc")]
        public JobTriggerCondition TriggerCondition { get; set; } = JobTriggerCondition.ALWAYS;
    }

    // Update DTO
    public class UpdateJobControlMappingDto
    {
        public bool? TargetState { get; set; }

        public JobTriggerCondition? TriggerCondition { get; set; }
    }
}
