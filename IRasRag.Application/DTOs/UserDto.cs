using System.ComponentModel.DataAnnotations;
using IRasRag.Application.Validators;

namespace IRasRag.Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class CreateUserDto
    {
        [Required(ErrorMessage = "Email không được để trống.")]
        [MaxLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [MaxLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự.")]
        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "Họ không được vượt quá 50 ký tự.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [PasswordComplexity]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vai trò người dùng không được để trống.")]
        public string RoleName { get; set; }
    }

    public class UpdateUserDto
    {

        [MaxLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string? Email { get; set; }

        [MaxLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự.")]
        public string? FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "Họ không được vượt quá 50 ký tự.")]
        public string? LastName { get; set; }

        [PasswordComplexity]
        public string? Password { get; set; }
        public string? RoleName { get; set; }
    }
}
