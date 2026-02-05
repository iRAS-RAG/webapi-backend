using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    public class UserFarmDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public Guid FarmId { get; set; }
        public string FarmName { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }

    // Create DTO
    public class CreateUserFarmDto
    {
        [Required(ErrorMessage = "UserId là bắt buộc")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "FarmId là bắt buộc")]
        public Guid FarmId { get; set; }
    }

    // Update DTO - For this entity, we typically don't update, but include for completeness
    public class UpdateUserFarmDto
    {
        // No fields to update - junction table typically delete and recreate
        // Including this for consistency with CRUD pattern
    }
}
