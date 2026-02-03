using System.ComponentModel.DataAnnotations;
using IRasRag.Application.Validators;

namespace IRasRag.Application.DTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email không được để trống.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        public string Password { get; set; }
    }

    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Email không được để trống.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mã đặt lại mật khẩu không được để trống.")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [PasswordComplexity]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống.")]
        public string ConfirmNewPassword { get; set; }
    }
}
