using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Common;

namespace IRasRag.Domain.Entities
{
    public class User : BaseEntity, ISoftDeletable
    {
        [Required]
        public Guid RoleId { get; set; }

        public Role Role { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? FirstName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        public ICollection<UserFarm> UserFarms { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}
