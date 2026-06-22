using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class TalentPoolCandidate : Audit
{
    public long TalentPoolCandidateId { get; set; }
    public long TalentPoolId { get; set; }
    public long CandidateId { get; set; }
    public int? Rank { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public TalentPool TalentPool { get; set; } = null!;
    public Candidate Candidate { get; set; } = null!;
}
