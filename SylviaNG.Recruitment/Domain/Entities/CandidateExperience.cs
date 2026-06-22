using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class CandidateExperience : Audit
{
    public long CandidateExperienceId { get; set; }
    public long CandidateId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string? Designation { get; set; }
    public string? Department { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrentJob { get; set; }
    public string? Responsibilities { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Candidate Candidate { get; set; } = null!;
}
