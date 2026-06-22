using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class InterviewEvaluation : Audit
{
    public long InterviewEvaluationId { get; set; }
    public long InterviewId { get; set; }
    public long PanelistUserId { get; set; }
    public decimal? OverallScore { get; set; }
    public InterviewResultEnum Recommendation { get; set; } = InterviewResultEnum.Pending;
    public string? Commentary { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Interview Interview { get; set; } = null!;
    public User PanelistUser { get; set; } = null!;
    public ICollection<InterviewEvaluationScore> Scores { get; set; } = new List<InterviewEvaluationScore>();
}
