using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// One weighted criterion row belonging to a Scorecard (EP-08 US-066). Every row has the same
/// columns (homogeneous), unlike ShortlistFilterCriterion's type-discriminator shape - mirrors
/// ExamQuestionOption's convention instead.
/// </summary>
public class ScorecardCriterion : Audit
{
    public long ScorecardCriterionId { get; set; }
    public long ScorecardId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public decimal MaxScore { get; set; }
    public int DisplayOrder { get; set; }

    // Navigation properties
    public Scorecard Scorecard { get; set; } = null!;
}
