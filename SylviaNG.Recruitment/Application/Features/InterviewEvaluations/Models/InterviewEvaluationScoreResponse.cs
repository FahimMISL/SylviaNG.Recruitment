namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models
{
    public class InterviewEvaluationScoreResponse
    {
        public long ScorecardCriterionId { get; set; }
        public string CriterionName { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public decimal MaxScore { get; set; }
        public decimal Score { get; set; }
    }
}
