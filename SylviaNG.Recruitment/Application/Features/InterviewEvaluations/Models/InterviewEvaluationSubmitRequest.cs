namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models
{
    /// <summary>Submitted by HR on the panelist's (EmployeeId) behalf - see feature doc for why
    /// there's no panelist self-service login/identity-resolution mechanism.</summary>
    public class InterviewEvaluationSubmitRequest
    {
        public long EmployeeId { get; set; }
        public long ScorecardId { get; set; }
        public List<InterviewEvaluationScoreRequest> Scores { get; set; } = new();
        public string? OverallComments { get; set; }
    }
}
