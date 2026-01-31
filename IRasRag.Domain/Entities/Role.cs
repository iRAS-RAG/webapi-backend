using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Common;

namespace IRasRag.Domain.Entities
{
    public class Role : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
    }
}
