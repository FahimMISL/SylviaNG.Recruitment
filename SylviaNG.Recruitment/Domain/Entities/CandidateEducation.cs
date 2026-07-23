using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class CandidateEducation : Audit
{
    public long CandidateEducationId { get; set; }
    public long CandidateProfileId { get; set; }

    // Degree (dynamic admin-managed lookup, replaces the old free-text DegreeTitle). Board is
    // only meaningful when the selected Degree.Position marks it SSC/HSC-equivalent (1 or 2) -
    // see Degree.Position for the grouping.
    public long DegreeId { get; set; }
    public long? EducationBoardId { get; set; }
    public string Institution { get; set; } = string.Empty;
    public long? UniversityLibraryItemId { get; set; }
    public EducationLevelEnum? EducationLevel { get; set; }
    public int PassingYear { get; set; }
    public GradingSystemEnum? GradingSystem { get; set; }
    public string Result { get; set; } = string.Empty;
    public string? MajorSubject { get; set; }

    public CandidateProfile CandidateProfile { get; set; } = null!;
    public Degree Degree { get; set; } = null!;
    public EducationBoard? EducationBoard { get; set; }
    public UniversityLibraryItem? UniversityLibraryItem { get; set; }
}
