using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// One criterion row within a ShortlistFilter (US-043 AC2). Only the field(s) relevant to
/// CriterionType are populated; the rest stay null. Typed nullable columns rather than a
/// generic key/value shape, matching PipelineStage's precedent in this codebase.
/// </summary>
public class ShortlistFilterCriterion : Audit
{
    public long ShortlistFilterCriterionId { get; set; }
    public long ShortlistFilterId { get; set; }
    public CriterionTypeEnum CriterionType { get; set; }
    public int DisplayOrder { get; set; }

    // CriterionType.EducationLevel
    public EducationLevelEnum? MinEducationLevel { get; set; }

    // CriterionType.MinExperienceYears
    public int? MinExperienceYears { get; set; }

    // CriterionType.RequiredSkills - comma-separated skill names, same convention as
    // PipelineStage.RequiredDocuments (descriptive metadata, not a relational child table).
    public string? RequiredSkillNames { get; set; }

    // CriterionType.AgeRange - either bound may be set independently of the other.
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }

    // CriterionType.District - case-insensitive substring match against the candidate's
    // free-text address (no structured district field exists on CandidateProfile).
    public string? RequiredDistrict { get; set; }

    // CriterionType.MinScreeningScore - compared against the latest AutoShortlistRun score for
    // the application (US-046). Unmet if no run has scored the application yet.
    public int? MinScreeningScore { get; set; }

    // Navigation properties
    public ShortlistFilter ShortlistFilter { get; set; } = null!;
}
