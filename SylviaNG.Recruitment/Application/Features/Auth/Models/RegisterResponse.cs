namespace SylviaNG.Recruitment.Application.Features.Auth.Models
{
    public class RegisterResponse
    {
        public string Email { get; set; } = string.Empty;

        /// <summary>True when the account must verify its email before it can log in (US-001 AC2).</summary>
        public bool RequiresEmailVerification { get; set; }
    }
}
