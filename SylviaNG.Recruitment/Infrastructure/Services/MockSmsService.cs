using Microsoft.Extensions.Logging;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Infrastructure.Services
{
    public class MockSmsService : ISmsService
    {
        private readonly ILogger<MockSmsService> _logger;

        public MockSmsService(ILogger<MockSmsService> logger)
        {
            _logger = logger;
        }

        public Task<bool> SendAsync(string phoneNumber, string message)
        {
            _logger.LogInformation("[MockSMS] To: {Phone} | Message: {Message}", phoneNumber, message);
            return Task.FromResult(true);
        }

        public Task<List<(string PhoneNumber, bool Success)>> SendBulkAsync(List<string> phoneNumbers, string message)
        {
            var results = new List<(string PhoneNumber, bool Success)>();
            foreach (var phone in phoneNumbers)
            {
                _logger.LogInformation("[MockSMS] Bulk To: {Phone} | Message: {Message}", phone, message);
                results.Add((phone, true));
            }
            return Task.FromResult(results);
        }
    }
}
