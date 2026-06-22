namespace SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Models
{
    public class ImpersonationLogCreateRequest
    {
        public long AdminUserId { get; set; }
        public long CandidateId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
    }
}
