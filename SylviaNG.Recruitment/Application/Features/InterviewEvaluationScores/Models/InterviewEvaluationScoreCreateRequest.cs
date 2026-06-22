namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Models
{
    public class InterviewEvaluationScoreCreateRequest
    {
        public long InterviewEvaluationId { get; set; }
        public long InterviewScorecardCriteriaId { get; set; }
        public int Score { get; set; }
        public string? Notes { get; set; }
    }
}
