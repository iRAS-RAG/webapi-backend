using IRasRag.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace IRasRag.Domain.Entities
{
    public class Job : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public Guid JobTypeId { get; set; }

        public Guid? SensorId { get; set; }

        public float? MinValue { get; set; }

        public float? MaxValue { get; set; }

        [Required]
        public bool DefaultState { get; set; } = false;

        [Required]
        public bool IsActive { get; set; } = true;

        public TimeSpan? StartTime { get; set; } // e.g., 05:00:00

        public TimeSpan? EndTime { get; set; } // e.g., 17:00:00

        public int? RepeatIntervalMinutes { get; set; } // e.g., 180 for every 3 hours

        [MaxLength(20)]
        public string ExecutionDays { get; set; } // e.g., "1,2,3,4,5" for Mon-Fri, or "ALL"

        [Required]
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public JobType JobType { get; set; }
        public Sensor Sensor { get; set; }
        public ICollection<JobControlMapping> JobControlMappings { get; set; }
    }
}
