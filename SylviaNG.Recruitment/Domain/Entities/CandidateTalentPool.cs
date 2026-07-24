using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A lightweight "saved for later" bucket HR can bulk-add CV Bank search results into
/// (US-045 AC5). One row per candidate; CreatedAt/CreatedBy (from Audit) record when/who
/// added them. Not tied to any vacancy - that's what a real shortlist is for.
/// </summary>
public class CandidateTalentPool : Audit
{
    public long CandidateTalentPoolId { get; set; }
    public long CandidateProfileId { get; set; }

    public CandidateProfile CandidateProfile { get; set; } = null!;
}
