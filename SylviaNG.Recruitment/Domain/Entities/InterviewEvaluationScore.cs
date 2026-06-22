using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class InterviewEvaluationScore : Audit
{
    public long InterviewEvaluationScoreId { get; set; }
    public long InterviewEvaluationId { get; set; }
    public long InterviewScorecardCriteriaId { get; set; }
    public int Score { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public InterviewEvaluation InterviewEvaluation { get; set; } = null!;
    public InterviewScorecardCriteria ScorecardCriteria { get; set; } = null!;
}
