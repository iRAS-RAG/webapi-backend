using IRasRag.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace IRasRag.Domain.Entities
{
    public class User : BaseEntity
    {
        [Required]
        public Guid RoleId { get; set; }

        public Role Role { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string UserName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? FirstName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        public bool IsVerified { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

    }
}
