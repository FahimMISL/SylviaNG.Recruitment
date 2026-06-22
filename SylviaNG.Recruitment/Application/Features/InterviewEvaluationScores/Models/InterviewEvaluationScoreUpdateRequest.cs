namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Models
{
    public class InterviewEvaluationScoreUpdateRequest
    {
        public long? InterviewEvaluationId { get; set; }
        public long? InterviewScorecardCriteriaId { get; set; }
        public int? Score { get; set; }
        public string? Notes { get; set; }
        public bool? IsActive { get; set; }
    }
}
