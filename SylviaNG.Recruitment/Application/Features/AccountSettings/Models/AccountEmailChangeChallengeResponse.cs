namespace SylviaNG.Recruitment.Application.Features.AccountSettings.Models
{
    public class AccountEmailChangeChallengeResponse
    {
        public string ChallengeId { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
    }
}
