namespace SylviaNG.Recruitment.Application.Features.Auth.Models
{
    public class ForgotPasswordResponse
    {
        public string ChallengeId { get; set; } = string.Empty;

        /// <summary>When the OTP code expires (UTC) - drives the code-entry screen's countdown.</summary>
        public DateTime ExpiresAtUtc { get; set; }
    }
}
