using IRasRag.Application.Common.Interfaces.Email;
using IRasRag.Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace IRasRag.Infrastructure.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailOption, ILogger<EmailService> logger)
        {
            _emailSettings = emailOption.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Recipient email is required", nameof(to));

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Email subject is required", nameof(subject));

            var message = new MimeMessage();
            message.From.Add(
                new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail)
            );
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            message.Body = new BodyBuilder { HtmlBody = body }.ToMessageBody();

            try
            {
                using var client = new SmtpClient();

                await client.ConnectAsync(
                    _emailSettings.SmtpServer,
                    _emailSettings.Port,
                    SecureSocketOptions.StartTls
                );

                client.AuthenticationMechanisms.Remove("XOAUTH2");

                await client.AuthenticateAsync(
                    _emailSettings.SenderEmail,
                    _emailSettings.SenderPassword
                );

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", to);
                throw;
            }
        }

        public async Task<string> GenerateResetPasswordEmailBodyAsync(
            string code,
            int expiresInMinutes
        )
        {
            var template = await LoadTemplateAsync("ResetPasswordTemplate.html");

            template = template
                .Replace("{{ResetCode}}", code)
                .Replace("{{ExpirationMinutes}}", expiresInMinutes.ToString());

            return template;
        }

        private async Task<string> LoadTemplateAsync(string templateName)
        {
            var assembly = typeof(EmailService).Assembly;

            var resourceName = "IRasRag.Infrastructure.EmailTemplates." + templateName;

            using var stream =
                assembly.GetManifestResourceStream(resourceName)
                ?? throw new FileNotFoundException($"Email template '{templateName}' not found.");

            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
}
