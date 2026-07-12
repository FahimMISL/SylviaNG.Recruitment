using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class CandidateEducation : Audit
{
    public long CandidateEducationId { get; set; }
    public long CandidateProfileId { get; set; }

    public string DegreeTitle { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public EducationLevelEnum? EducationLevel { get; set; }
    public int PassingYear { get; set; }
    public string Result { get; set; } = string.Empty;
    public string? MajorSubject { get; set; }

    public CandidateProfile CandidateProfile { get; set; } = null!;
}
