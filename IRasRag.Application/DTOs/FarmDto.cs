using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    public class FarmDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

    public class CreateFarmDto
    {
        [Required(ErrorMessage = "Tên trang trại không được để trống.")]
        [MaxLength(255, ErrorMessage = "Tên trang trại không được vượt quá 255 ký tự.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống.")]
        [MaxLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [MaxLength(50, ErrorMessage = "Số điện thoại không được vượt quá 50 ký tự.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email không được để trống.")]
        [MaxLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }
    }

    public class UpdateFarmDto
    {
        [MaxLength(255, ErrorMessage = "Tên trang trại không được vượt quá 255 ký tự.")]
        public string? Name { get; set; }

        [MaxLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự.")]
        public string? Address { get; set; }

        [MaxLength(50, ErrorMessage = "Số điện thoại không được vượt quá 50 ký tự.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string? PhoneNumber { get; set; }

        [MaxLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string? Email { get; set; }
    }
}
