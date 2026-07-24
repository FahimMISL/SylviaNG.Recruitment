using Microsoft.Extensions.Logging;
using SylviaNG.Recruitment.Application.Common.Email;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Common.Notifications
{
    /// <summary>
    /// EP-09 US-077: bounded in-process retry wrapper around ISmtpEmailService.TrySendAsync -
    /// no durable retry-queue infrastructure exists anywhere in this codebase (no Hangfire/Polly/
    /// Quartz), so a fixed-delay bounded retry at send time is the consistent choice here rather
    /// than introducing new background-worker plumbing for one email path.
    /// </summary>
    public static class EmailRetrySender
    {
        public static async Task<EmailSendResult> SendWithRetryAsync(
            ISmtpEmailService smtpEmailService,
            EmailMessage message,
            ILogger logger,
            int maxAttempts = 3,
            TimeSpan? delay = null,
            CancellationToken cancellationToken = default)
        {
            var effectiveDelay = delay ?? TimeSpan.FromSeconds(2);
            EmailSendResult result = new() { Success = false, ErrorMessage = "No send attempt was made." };

            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                result = await smtpEmailService.TrySendAsync(message, cancellationToken);
                if (result.Success)
                    return result;

                logger.LogWarning(
                    "Email send attempt {Attempt}/{MaxAttempts} failed to {To}: {Error}",
                    attempt, maxAttempts, message.To, result.ErrorMessage);

                if (attempt < maxAttempts)
                    await Task.Delay(effectiveDelay, cancellationToken);
            }

            return result;
        }
    }
}
