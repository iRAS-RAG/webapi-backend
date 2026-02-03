using IRasRag.Application.Validators;

namespace IRasRag.Application.DTOs
{
    public class LoginRequest
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string UserName { get; set; } = string.Empty;

        public string? FirstName { get; set; } = string.Empty;

        public string? LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [PasswordComplexity]
        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
