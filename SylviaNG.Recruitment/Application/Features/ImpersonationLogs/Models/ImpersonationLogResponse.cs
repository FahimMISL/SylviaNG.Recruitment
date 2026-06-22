namespace SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Models
{
    public class ImpersonationLogResponse
    {
        public long ImpersonationLogId { get; set; }
        public long AdminUserId { get; set; }
        public long CandidateId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
