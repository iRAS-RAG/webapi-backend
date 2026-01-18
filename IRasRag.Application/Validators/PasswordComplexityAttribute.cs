using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.Validators
{
    public class PasswordComplexityAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            var specialChars = "!@#$%^&*()-_=+[]{}|;:'\",.<>?/`~";
            if (value == null)
                return new ValidationResult("Mật khẩu không được để trống.");

            var password = value.ToString()!;
            if (
                password.Length < 8
                || !password.Any(char.IsLower)
                || !password.Any(char.IsUpper)
                || !password.Any(char.IsDigit)
                || !password.Any(c => specialChars.Contains(c))
            )
            {
                return new ValidationResult(
                    "Mật khẩu phải có ít nhất 8 ký tự, 1 chữ thường, 1 chữ hoa, 1 số và 1 ký tự đặc biệt."
                );
            }

            return ValidationResult.Success;
        }
    }
}
