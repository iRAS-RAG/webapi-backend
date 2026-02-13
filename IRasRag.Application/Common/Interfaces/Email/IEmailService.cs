namespace IRasRag.Application.Common.Interfaces.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task<string> GenerateResetPasswordEmailBodyAsync(string code, int expiresInMinutes);
        Task<string> GenerateAccountCreatedEmailBodyAsync(
            string roleName,
            string email,
            string plainPassword
        );
    }
}
