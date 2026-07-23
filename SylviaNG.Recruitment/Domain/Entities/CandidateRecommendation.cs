using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// HR's recommendation to fast-track a shortlisted candidate to final selection (US-049),
/// pending review by the Hiring Manager (the existing Admin role - no separate Hiring Manager
/// role exists in this codebase's 3-account auth scheme, per user decision).
/// </summary>
public class CandidateRecommendation : Audit
{
    public long CandidateRecommendationId { get; set; }
    public long JobApplicationId { get; set; }
    public string Justification { get; set; } = string.Empty;
    public string RecommendedByUserName { get; set; } = string.Empty;
    public DateTime RecommendedAt { get; set; }

    public new RecommendationStatusEnum Status { get; set; } = RecommendationStatusEnum.Pending;
    public string? ReviewComments { get; set; }
    public string? ReviewedByUserName { get; set; }
    public DateTime? ReviewedAt { get; set; }

    public JobApplication JobApplication { get; set; } = null!;
}
