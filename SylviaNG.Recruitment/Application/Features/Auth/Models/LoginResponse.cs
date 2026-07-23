namespace SylviaNG.Recruitment.Application.Features.Auth.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }

        /// <summary>Null for offline-fallback sessions - only Keycloak issues refresh tokens.</summary>
        public string? RefreshToken { get; set; }
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
