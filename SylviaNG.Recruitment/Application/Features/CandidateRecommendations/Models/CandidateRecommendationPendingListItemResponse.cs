namespace SylviaNG.Recruitment.Application.Features.CandidateRecommendations.Models
{
    /// <summary>One row in the Hiring Manager's pending-recommendations review queue (AC3).</summary>
    public class CandidateRecommendationPendingListItemResponse
    {
        public long CandidateRecommendationId { get; set; }
        public long JobApplicationId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string? JobPostingTitle { get; set; }
        public string Justification { get; set; } = string.Empty;
        public string RecommendedByUserName { get; set; } = string.Empty;
        public DateTime RecommendedAt { get; set; }
    }
}
