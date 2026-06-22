using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class CandidateSkill : Audit
{
    public long CandidateSkillId { get; set; }
    public long CandidateId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public string? ProficiencyLevel { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Candidate Candidate { get; set; } = null!;
}
