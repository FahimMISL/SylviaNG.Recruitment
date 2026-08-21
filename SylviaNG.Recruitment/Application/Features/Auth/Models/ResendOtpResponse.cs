namespace SylviaNG.Recruitment.Application.Features.Auth.Models
{
    public class ResendOtpResponse
    {
        /// <summary>When the newly-sent OTP code expires (UTC) - drives the countdown restart on
        /// the code-entry screen.</summary>
        public DateTime ExpiresAtUtc { get; set; }
    }
}
