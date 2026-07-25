using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Membership of a candidate in a named talent pool (US-039 AC1/AC5). Deliberately has no
/// reverse nav property on CandidateProfile - the badge/filter lookups join through this
/// repository directly, the same loose-coupling precedent CandidateProfileService already
/// uses for ApplicationHistory (IJobApplicationRepository.GetByCandidateEmailAsync).
/// </summary>
public class TalentPoolCandidate : Audit
{
    public long TalentPoolCandidateId { get; set; }
    public long TalentPoolId { get; set; }
    public long CandidateProfileId { get; set; }
    public DateTime AddedDate { get; set; }

    public TalentPool TalentPool { get; set; } = null!;
    public CandidateProfile CandidateProfile { get; set; } = null!;
}
