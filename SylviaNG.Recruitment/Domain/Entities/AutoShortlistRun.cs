using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// One "Run AI/Manual Shortlisting" pass for a job posting (US-046). Persisted so HR can revisit
/// the ranked list, adjust the cutoff, or override individual decisions without re-scoring -
/// re-running always creates a fresh run rather than mutating an existing one.
/// </summary>
public class AutoShortlistRun : Audit
{
    public long AutoShortlistRunId { get; set; }
    public long JobPostingId { get; set; }

    /// <summary>Which IShortlistScoringService implementation actually ran ("Manual" or "Ai") - for HR transparency.</summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>0-100. Mutable after the run (AC3/AC5) - pass/fail is recomputed from this, never stamped per row.</summary>
    public int CutoffScore { get; set; }

    public DateTime RunAt { get; set; }

    public ICollection<AutoShortlistResult> Results { get; set; } = new List<AutoShortlistResult>();
}
