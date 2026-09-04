namespace SylviaNG.Recruitment.Application.Features.Auth.Models
{
    public class ResetPasswordRequest
    {
        public string ChallengeId { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
