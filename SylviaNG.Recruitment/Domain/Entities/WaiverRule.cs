using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Admin-managed rule (EP-17/US-127) that waives the application fee when a submitting
/// candidate matches its criteria. Each criterion is nullable = wildcard ("Any"); a rule with
/// every criterion null matches every candidate. Evaluated in JobApplicationService.SubmitAsync,
/// ordered by Priority ascending (lower = evaluated first), first fully-matching active rule wins.
/// </summary>
public class WaiverRule : Audit
{
    public long WaiverRuleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>Null = matches any candidate type. Otherwise must equal CandidateProfile.IsInternal.</summary>
    public WaiverCandidateTypeEnum? CandidateTypeFilter { get; set; }

    /// <summary>Null = matches any special category (including none selected).</summary>
    public long? SpecialCategoryId { get; set; }

    /// <summary>Null = matches any referral source (including none selected).</summary>
    public long? ReferralSourceId { get; set; }

    public int Priority { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public SpecialCategory? SpecialCategory { get; set; }
    public ReferralSource? ReferralSource { get; set; }
}
