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

        /// <summary>EP-09 Feature 2: true when Token/RefreshToken are withheld pending OTP
        /// verification (candidate login only, gate enabled). False for every existing
        /// Admin/HR login and whenever the gate is disabled - no behavior change for them.</summary>
        public bool RequiresOtp { get; set; }

        /// <summary>Opaque id to pass to verify-otp/resend-otp. Only set when RequiresOtp is true.</summary>
        public string? ChallengeId { get; set; }

        /// <summary>When the current OTP code expires (UTC). Only set when RequiresOtp is true -
        /// drives the countdown shown on the code-entry screen.</summary>
        public DateTime? OtpExpiresAtUtc { get; set; }
    }
}
