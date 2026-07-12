using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class CandidateWorkExperience : Audit
{
    public long CandidateWorkExperienceId { get; set; }
    public long CandidateProfileId { get; set; }

    public string CompanyName { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string Responsibilities { get; set; } = string.Empty;
    public string? Location { get; set; }

    public CandidateProfile CandidateProfile { get; set; } = null!;
}
