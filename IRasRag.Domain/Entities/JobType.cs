using IRasRag.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace IRasRag.Domain.Entities
{
    public class JobType : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        // Navigation properties
        public ICollection<Job> Jobs { get; set; }
    }
}
