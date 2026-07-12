namespace SylviaNG.Recruitment.Application.Features.AccountSettings.Models
{
    public class AccountSettingsResponse
    {
        public string Email { get; set; } = string.Empty;
        public string? ProfilePhotoPath { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
