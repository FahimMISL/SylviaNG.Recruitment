namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models
{
    public class InterviewEvaluationResponse
    {
        public long InterviewEvaluationId { get; set; }
        public long InterviewId { get; set; }
        public long EmployeeId { get; set; }
        public long ScorecardId { get; set; }
        public string ScorecardName { get; set; } = string.Empty;
        public string? OverallComments { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string? SubmittedByUserName { get; set; }
        public List<InterviewEvaluationScoreResponse> Scores { get; set; } = new();

        /// <summary>Not persisted - Σ(Score/MaxScore × Weight) / Σ(Weight) × 100, robust regardless
        /// of whether the scorecard's weights sum to exactly 100.</summary>
        public decimal WeightedScore { get; set; }
    }
}
