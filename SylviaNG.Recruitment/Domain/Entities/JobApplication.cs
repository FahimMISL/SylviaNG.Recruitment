using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class JobApplication : Audit
{
    public long JobApplicationId { get; set; }
    public long JobPostingId { get; set; }
    public long? CandidateId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string? CandidateEmail { get; set; }
    public string? CandidatePhone { get; set; }
    public string? ResumeUrl { get; set; }
    public string? CoverLetter { get; set; }
    public ApplicationStatusEnum ApplicationStatus { get; set; } = ApplicationStatusEnum.Applied;
    public ReferralSourceEnum? Source { get; set; }
    public DateTime? AppliedDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ProfileSnapshotJson { get; set; }

    // Navigation properties
    public JobPosting JobPosting { get; set; } = null!;
    public Candidate? Candidate { get; set; }
    public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
    public ApplicationScreeningResult? ScreeningResult { get; set; }
}
