using System.ComponentModel.DataAnnotations;

namespace IRasRag.Domain.Common
{
    public abstract class BaseEntity
    {
        [Required]
        public Guid Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
