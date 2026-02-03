using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Common;

namespace IRasRag.Domain.Entities
{
    public class FishTank : BaseEntity, ISoftDeletable
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public float Height { get; set; }

        [Required]
        public float Radius { get; set; }

        [Required]
        public Guid FarmId { get; set; }

        [MaxLength(50)]
        public string TopicCode { get; set; }

        [Required]
        [MaxLength(255)]
        public string CameraUrl { get; set; } = string.Empty;

        [Required]
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public Farm Farm { get; set; }
        public ICollection<FarmingBatch> FarmingBatches { get; set; }
        public ICollection<MasterBoard> MasterBoards { get; set; }
        public ICollection<Alert> Alerts { get; set; }
    }
}
