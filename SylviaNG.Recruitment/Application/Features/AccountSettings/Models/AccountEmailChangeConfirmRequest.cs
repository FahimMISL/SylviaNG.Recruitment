namespace SylviaNG.Recruitment.Application.Features.AccountSettings.Models
{
    public class AccountEmailChangeConfirmRequest
    {
        public string ChallengeId { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
    }
}
