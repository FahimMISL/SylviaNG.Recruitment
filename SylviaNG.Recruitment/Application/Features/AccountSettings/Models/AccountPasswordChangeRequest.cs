namespace SylviaNG.Recruitment.Application.Features.AccountSettings.Models
{
    public class AccountPasswordChangeRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
