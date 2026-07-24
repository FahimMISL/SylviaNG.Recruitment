using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// One candidate's score within an AutoShortlistRun (US-046). Passed/FinalIncluded are computed
/// from Score vs. the parent run's CutoffScore, plus HrOverrideDecision when set - never stored,
/// so adjusting the cutoff is a single-row update, not a rewrite of every result.
/// </summary>
public class AutoShortlistResult : Audit
{
    public long AutoShortlistResultId { get; set; }
    public long AutoShortlistRunId { get; set; }
    public long JobApplicationId { get; set; }

    public int? Score { get; set; }
    public string? Explanation { get; set; }

    /// <summary>Comma-separated candidate skills matched against the posting's Requirements/Description
    /// (US-037 AC2) - computed the same way regardless of Manual/Ai provider, see CandidateMatchAnalyzer.</summary>
    public string? MatchedSkills { get; set; }

    /// <summary>Human-readable experience bucket, e.g. "3-5 years" (US-037 AC2).</summary>
    public string? ExperienceBand { get; set; }

    public bool ScoringFailed { get; set; }
    public string? ScoringError { get; set; }

    /// <summary>Null = follow the computed pass/fail; set = HR override (AC5).</summary>
    public HrOverrideDecisionEnum? HrOverrideDecision { get; set; }

    public AutoShortlistRun AutoShortlistRun { get; set; } = null!;
}
