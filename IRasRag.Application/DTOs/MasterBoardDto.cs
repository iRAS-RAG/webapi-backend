using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    public class MasterBoardDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string MacAddress { get; set; }
        public Guid FishTankId { get; set; }
        public string FishTankName { get; set; }
    }

    // Create DTO
    public class CreateMasterBoardDto
    {
        [Required(ErrorMessage = "Tên bảng mạch là bắt buộc")]
        [MaxLength(255, ErrorMessage = "Tên bảng mạch không được vượt quá 255 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Địa chỉ MAC là bắt buộc")]
        [MaxLength(50, ErrorMessage = "Địa chỉ MAC không được vượt quá 50 ký tự")]
        public string MacAddress { get; set; }

        [Required(ErrorMessage = "Id hồ cá là bắt buộc")]
        public Guid FishTankId { get; set; }
    }

    // Update DTO
    public class UpdateMasterBoardDto
    {
        [MaxLength(255, ErrorMessage = "Tên bảng mạch không được vượt quá 255 ký tự")]
        public string? Name { get; set; }

        [MaxLength(50, ErrorMessage = "Địa chỉ MAC không được vượt quá 50 ký tự")]
        public string? MacAddress { get; set; }

        public Guid? FishTankId { get; set; }
    }
}
