namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Models
{
    public class InterviewEvaluationScoreResponse
    {
        public long InterviewEvaluationScoreId { get; set; }
        public long InterviewEvaluationId { get; set; }
        public long InterviewScorecardCriteriaId { get; set; }
        public int Score { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
    }
}
