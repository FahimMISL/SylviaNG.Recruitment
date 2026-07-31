using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CandidateRecommendations.Models
{
    public class CandidateRecommendationResponse
    {
        public long CandidateRecommendationId { get; set; }
        public long JobApplicationId { get; set; }
        public string Justification { get; set; } = string.Empty;
        public string RecommendedByUserName { get; set; } = string.Empty;
        public DateTime RecommendedAt { get; set; }
        public RecommendationStatusEnum Status { get; set; }
        public string? ReviewComments { get; set; }
        public string? ReviewedByUserName { get; set; }
        public DateTime? ReviewedAt { get; set; }
    }
}
