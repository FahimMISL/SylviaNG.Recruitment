using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class CandidateSkill : Audit
{
    public long CandidateSkillId { get; set; }
    public long CandidateProfileId { get; set; }

    public string SkillName { get; set; } = string.Empty;
    public long? SkillLibraryItemId { get; set; }
    public string? ProficiencyLevel { get; set; }

    public CandidateProfile CandidateProfile { get; set; } = null!;
    public SkillLibraryItem? SkillLibraryItem { get; set; }
}
