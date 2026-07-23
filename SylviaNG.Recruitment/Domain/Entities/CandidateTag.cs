using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// HR-only free-text tag on a candidate's profile (US-041) - never visible to or editable by
/// the candidate. Distinct from CandidateSkill, which is candidate-owned self-service data.
/// </summary>
public class CandidateTag : Audit
{
    public long CandidateTagId { get; set; }
    public long CandidateProfileId { get; set; }

    public string TagName { get; set; } = string.Empty;

    public CandidateProfile CandidateProfile { get; set; } = null!;
}
