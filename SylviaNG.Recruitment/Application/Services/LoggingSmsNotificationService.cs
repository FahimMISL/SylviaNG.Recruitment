using Microsoft.Extensions.Logging;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Services
{
    /// <summary>
    /// Stub SMS gateway (US-055/US-056) - no real gateway is wired up yet, this just logs what
    /// would have been sent and always reports success so enrollment notification flow can be
    /// built and tested end-to-end ahead of a real provider integration.
    /// </summary>
    public class LoggingSmsNotificationService : ISmsNotificationService
    {
        private readonly ILogger<LoggingSmsNotificationService> _logger;

        public LoggingSmsNotificationService(ILogger<LoggingSmsNotificationService> logger)
        {
            _logger = logger;
        }

        public Task<bool> TrySendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[SMS-STUB] Would send SMS to {Phone}: {Message}", phoneNumber, message);
            return Task.FromResult(true);
        }
    }
}
