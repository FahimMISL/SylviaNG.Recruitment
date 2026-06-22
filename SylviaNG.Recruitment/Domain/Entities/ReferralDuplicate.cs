using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class ReferralDuplicate : Audit
{
    public long ReferralDuplicateId { get; set; }
    public long PrimaryReferralId { get; set; }
    public long DuplicateReferralId { get; set; }
    public string MatchField { get; set; } = string.Empty;
    public DuplicateResolutionEnum Resolution { get; set; } = DuplicateResolutionEnum.Unresolved;
    public long? ResolvedByUserId { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Referral PrimaryReferral { get; set; } = null!;
    public Referral DuplicateReferral { get; set; } = null!;
    public User? ResolvedByUser { get; set; }
}
