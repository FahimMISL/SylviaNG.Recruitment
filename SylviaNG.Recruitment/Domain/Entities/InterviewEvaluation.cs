using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// One panelist's scorecard evaluation of an Interview (EP-08 US-067). EmployeeId is the panelist
/// being scored - must be one of the Interview's own PanelMembers - but the score is entered by
/// HR on that panelist's behalf (no panelist login/identity-resolution mechanism exists in this
/// codebase - see feature doc). One row per (InterviewId, EmployeeId) pair.
/// </summary>
public class InterviewEvaluation : Audit
{
    public long InterviewEvaluationId { get; set; }
    public long InterviewId { get; set; }
    public long ScorecardId { get; set; }
    public long EmployeeId { get; set; }
    public string? OverallComments { get; set; }
    public DateTime SubmittedAt { get; set; }
    public string? SubmittedByUserName { get; set; }

    // Navigation properties
    public Interview Interview { get; set; } = null!;
    public Scorecard Scorecard { get; set; } = null!;
    public ICollection<InterviewEvaluationScore> Scores { get; set; } = new List<InterviewEvaluationScore>();
}
