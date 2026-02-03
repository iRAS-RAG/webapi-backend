namespace IRasRag.Application.Common.Interfaces.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task<string> GenerateResetPasswordEmailBodyAsync(string code, int expiresInMinutes);
        //Task SendTemplatedEmailJob(string recipientEmail, string subject, string body);
    }
}
