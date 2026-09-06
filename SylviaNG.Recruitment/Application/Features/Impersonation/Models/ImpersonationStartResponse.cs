namespace SylviaNG.Recruitment.Application.Features.Impersonation.Models
{
    public class ImpersonationStartResponse
    {
        public long ImpersonationSessionId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
        public string TargetFullName { get; set; } = string.Empty;
        public string TargetEmail { get; set; } = string.Empty;
        public string TargetRole { get; set; } = string.Empty;
    }
}
