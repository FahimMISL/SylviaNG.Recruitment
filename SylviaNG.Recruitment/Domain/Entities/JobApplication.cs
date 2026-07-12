using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Represents a candidate's application to a job posting.
/// </summary>
public class JobApplication : Audit
{
    public long JobApplicationId { get; set; }
    public long JobPostingId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string? CandidateEmail { get; set; }
    public string? CandidatePhone { get; set; }
    public string? ResumeUrl { get; set; }
    public string? CoverLetter { get; set; }
    public ApplicationStatusEnum ApplicationStatus { get; set; } = ApplicationStatusEnum.Applied;
    public DateTime? AppliedDate { get; set; }
    public bool IsActive { get; set; } = true;
    public ApplicationSourceEnum Source { get; set; } = ApplicationSourceEnum.External;

    // Navigation properties
    public JobPosting JobPosting { get; set; } = null!;
    public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
    public ICollection<ApplicationStatusHistory> StatusHistory { get; set; } = new List<ApplicationStatusHistory>();
}
