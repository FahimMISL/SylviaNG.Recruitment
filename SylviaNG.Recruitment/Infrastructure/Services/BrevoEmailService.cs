using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Email;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Infrastructure.Services
{
    /// <summary>
    /// Brevo's HTTP API (https://api.brevo.com/v3/smtp/email) instead of raw SMTP - some hosts
    /// (Render's free tier included) block outbound SMTP ports at the network level, which no
    /// SMTP config can work around. This sends over plain HTTPS instead. Brevo's free tier (300
    /// emails/day) needs only a single-sender email-click verification, no DNS/domain, and sends
    /// to any recipient. Same never-throws contract as SmtpEmailService: every failure path
    /// returns a result rather than propagating.
    /// </summary>
    public class BrevoEmailService : ISmtpEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly BrevoSettings _settings;
        private readonly ILogger<BrevoEmailService> _logger;

        public BrevoEmailService(HttpClient httpClient, IOptions<BrevoSettings> options, ILogger<BrevoEmailService> logger)
        {
            _httpClient = httpClient;
            _settings = options.Value;
            _logger = logger;
        }

        public async Task<EmailSendResult> TrySendAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_settings.ApiKey) || string.IsNullOrWhiteSpace(_settings.FromEmail))
            {
                _logger.LogWarning("Brevo not configured - skipping email send to {To}.", message.To);
                return new EmailSendResult { Success = false, ErrorMessage = "Brevo not configured" };
            }

            try
            {
                var payload = new Dictionary<string, object>
                {
                    ["sender"] = new { name = _settings.FromName, email = _settings.FromEmail },
                    ["to"] = new[] { new { email = message.To } },
                    ["subject"] = message.Subject,
                    ["htmlContent"] = PlainTextEmailFormatter.ToHtml(message.HtmlBody)
                };

                if (message.Attachments.Count > 0)
                {
                    payload["attachment"] = message.Attachments
                        .Select(a => new
                        {
                            content = Convert.ToBase64String(a.Content),
                            name = a.FileName
                        })
                        .ToArray();
                }

                using var request = new HttpRequestMessage(HttpMethod.Post, "v3/smtp/email")
                {
                    Content = JsonContent.Create(payload)
                };
                request.Headers.Add("api-key", _settings.ApiKey);

                using var response = await _httpClient.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return new EmailSendResult { Success = true };
                }

                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Brevo API returned {StatusCode} sending to {To}: {Body}", response.StatusCode, message.To, body);
                return new EmailSendResult { Success = false, ErrorMessage = $"Brevo API returned {response.StatusCode}" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To} via Brevo.", message.To);
                return new EmailSendResult { Success = false, ErrorMessage = ex.Message };
            }
        }
    }
}
