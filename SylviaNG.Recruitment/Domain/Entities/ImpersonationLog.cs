using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class ImpersonationLog : Audit
{
    public long ImpersonationLogId { get; set; }
    public long AdminUserId { get; set; }
    public long CandidateId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public User AdminUser { get; set; } = null!;
    public Candidate Candidate { get; set; } = null!;
}
