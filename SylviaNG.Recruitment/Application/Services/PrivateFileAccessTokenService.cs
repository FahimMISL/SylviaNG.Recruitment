using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using System.Security.Cryptography;
using System.Text;

namespace SylviaNG.Recruitment.Application.Services
{
    public class PrivateFileAccessTokenService : IPrivateFileAccessTokenService
    {
        private readonly PrivateFileAccessSettings _settings;

        public PrivateFileAccessTokenService(IOptions<PrivateFileAccessSettings> options)
        {
            _settings = options.Value;
        }

        public string GenerateToken(string key, DateTimeOffset expiresAtUtc)
        {
            return Convert.ToHexString(ComputeHash(key, expiresAtUtc));
        }

        public bool IsValid(string key, string? token, DateTimeOffset expiresAtUtc)
        {
            if (string.IsNullOrWhiteSpace(token) || expiresAtUtc < DateTimeOffset.UtcNow)
                return false;

            byte[] provided;
            try
            {
                provided = Convert.FromHexString(token);
            }
            catch (FormatException)
            {
                return false;
            }

            var expected = ComputeHash(key, expiresAtUtc);
            return provided.Length == expected.Length && CryptographicOperations.FixedTimeEquals(provided, expected);
        }

        private byte[] ComputeHash(string key, DateTimeOffset expiresAtUtc)
        {
            if (string.IsNullOrWhiteSpace(_settings.SigningKey))
                throw new InvalidOperationException(
                    "PrivateFileAccess:SigningKey is not configured. Set it via 'dotnet user-secrets set \"PrivateFileAccess:SigningKey\" \"<value>\"' for local development, or your secret store in other environments.");

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_settings.SigningKey));
            var payload = $"{key}|{expiresAtUtc.ToUnixTimeSeconds()}";
            return hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        }
    }
}
