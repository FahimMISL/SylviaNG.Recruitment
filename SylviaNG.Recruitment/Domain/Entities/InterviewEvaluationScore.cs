using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>One scorecard criterion's score within an InterviewEvaluation (EP-08 US-067).</summary>
public class InterviewEvaluationScore : Audit
{
    public long InterviewEvaluationScoreId { get; set; }
    public long InterviewEvaluationId { get; set; }
    public long ScorecardCriterionId { get; set; }
    public decimal Score { get; set; }

    // Navigation properties
    public InterviewEvaluation InterviewEvaluation { get; set; } = null!;
    public ScorecardCriterion ScorecardCriterion { get; set; } = null!;
}
