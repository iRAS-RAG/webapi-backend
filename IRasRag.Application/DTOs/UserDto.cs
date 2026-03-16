using System.ComponentModel.DataAnnotations;
using IRasRag.Application.Validators;

namespace IRasRag.Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class UserProfileDto
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

        [Required(ErrorMessage = "Vai trò người dùng không được để trống.")]
        public string RoleName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [PasswordComplexity]
        public string Password { get; set; }
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
        public bool? IsDeleted { get; set; }
    }

    public class CreateOperatorUserDto
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
    }

    public class UpdateUserProfileDto
    {
        [MaxLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string? Email { get; set; }

        [MaxLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự.")]
        public string? FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "Họ không được vượt quá 50 ký tự.")]
        public string? LastName { get; set; }
    }

    public class UpdateUserPasswordDto
    {
        [Required(ErrorMessage = "Mật khẩu cũ không được để trống.")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được để trống.")]
        [PasswordComplexity]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu mới không được để trống.")]
        public string ConfirmNewPassword { get; set; }
    }

    // List Request DTO
    public class UserListRequest : BasePaginatedListRequest
    {
        public bool? IsDeleted { get; set; }
    }
}
