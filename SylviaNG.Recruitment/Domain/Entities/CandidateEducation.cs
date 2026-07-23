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

    // Major Subject (dynamic admin-managed lookup, replaces the old free-text MajorSubject
    // field). Mirrors the EducationBoardId/UniversityLibraryItemId pair already on this entity:
    // exactly one of these three is populated at a time, selected by the same Degree.Position
    // SSC/HSC-vs-university split the Board field already uses. MajorSubjectOtherText is shared
    // by both dropdowns' "Other (please specify)" sentinel (a frontend-only option, not a seeded
    // row in either lookup table).
    public long? MajorSubjectSscHscId { get; set; }
    public long? MajorSubjectUniversityId { get; set; }
    public string? MajorSubjectOtherText { get; set; }

    public CandidateProfile CandidateProfile { get; set; } = null!;
    public Degree Degree { get; set; } = null!;
    public EducationBoard? EducationBoard { get; set; }
    public UniversityLibraryItem? UniversityLibraryItem { get; set; }
    public MajorSubjectSscHsc? MajorSubjectSscHsc { get; set; }
    public MajorSubjectUniversity? MajorSubjectUniversity { get; set; }

    /// <summary>Resolves whichever of the three Major Subject fields is populated, for CV PDF
    /// generation and CV Bank search text - not mapped, computed on read.</summary>
    public string? MajorSubjectDisplay => MajorSubjectSscHsc?.Name ?? MajorSubjectUniversity?.Name ?? MajorSubjectOtherText;
}
