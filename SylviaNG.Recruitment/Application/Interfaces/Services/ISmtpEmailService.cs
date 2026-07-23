using SylviaNG.Recruitment.Application.Common.Email;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>Never throws - always returns an EmailSendResult, even when SMTP isn't configured or the send fails.</summary>
    public interface ISmtpEmailService
    {
        Task<EmailSendResult> TrySendAsync(EmailMessage message, CancellationToken cancellationToken = default);
    }
}
