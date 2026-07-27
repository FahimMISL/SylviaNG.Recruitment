using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CandidateRecommendations.Models
{
    public class CandidateRecommendationReviewRequest
    {
        /// <summary>Must be Accepted or Rejected - Pending is not a valid review outcome.</summary>
        public RecommendationStatusEnum Status { get; set; }
        public string? ReviewComments { get; set; }
    }
}
