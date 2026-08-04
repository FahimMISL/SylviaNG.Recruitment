namespace SylviaNG.Recruitment.Application.Features.Auth.Models
{
    public class VerifyOtpRequest
    {
        public string ChallengeId { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
