using SylviaNG.Recruitment.Application.Features.CandidateRecommendations.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class CandidateRecommendationMapper
    {
        public static CandidateRecommendationResponse ToResponse(this CandidateRecommendation entity)
        {
            return new CandidateRecommendationResponse
            {
                CandidateRecommendationId = entity.CandidateRecommendationId,
                JobApplicationId = entity.JobApplicationId,
                Justification = entity.Justification,
                RecommendedByUserName = entity.RecommendedByUserName,
                RecommendedAt = entity.RecommendedAt,
                Status = entity.Status,
                ReviewComments = entity.ReviewComments,
                ReviewedByUserName = entity.ReviewedByUserName,
                ReviewedAt = entity.ReviewedAt,
            };
        }

        public static CandidateRecommendationPendingListItemResponse ToPendingListItemResponse(this CandidateRecommendation entity)
        {
            return new CandidateRecommendationPendingListItemResponse
            {
                CandidateRecommendationId = entity.CandidateRecommendationId,
                JobApplicationId = entity.JobApplicationId,
                CandidateName = entity.JobApplication.CandidateName,
                JobPostingTitle = entity.JobApplication.JobPosting?.Title,
                Justification = entity.Justification,
                RecommendedByUserName = entity.RecommendedByUserName,
                RecommendedAt = entity.RecommendedAt,
            };
        }
    }
}
