using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using SylviaNG.Recruitment.Application.Common.Email;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Infrastructure.Services
{
    /// <summary>
    /// MailKit-backed SMTP sender (US-055/US-056 exam notifications). Deliberately never throws -
    /// the caller (ExamNotificationService) persists the outcome onto the enrollment row rather
    /// than reacting to an exception, so every failure path here returns a result instead.
    /// </summary>
    public class SmtpEmailService : ISmtpEmailService
    {
        private readonly SmtpSettings _settings;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IOptions<SmtpSettings> options, ILogger<SmtpEmailService> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task<EmailSendResult> TrySendAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            if (!_settings.IsEnabled || string.IsNullOrWhiteSpace(_settings.Host))
            {
                _logger.LogWarning("SMTP not configured - skipping email send to {To}.", message.To);
                return new EmailSendResult { Success = false, ErrorMessage = "SMTP not configured" };
            }

            try
            {
                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
                mimeMessage.To.Add(MailboxAddress.Parse(message.To));
                mimeMessage.Subject = message.Subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = message.HtmlBody };
                foreach (var attachment in message.Attachments)
                {
                    bodyBuilder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(attachment.ContentType));
                }
                mimeMessage.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                var secureSocketOptions = _settings.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
                await client.ConnectAsync(_settings.Host, _settings.Port, secureSocketOptions, cancellationToken);

                if (!string.IsNullOrWhiteSpace(_settings.Username))
                {
                    await client.AuthenticateAsync(_settings.Username, _settings.Password ?? string.Empty, cancellationToken);
                }

                await client.SendAsync(mimeMessage, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);

                return new EmailSendResult { Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}.", message.To);
                return new EmailSendResult { Success = false, ErrorMessage = ex.Message };
            }
        }
    }
}
