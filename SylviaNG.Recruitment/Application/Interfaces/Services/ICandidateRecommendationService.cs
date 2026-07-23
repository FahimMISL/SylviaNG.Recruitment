using SylviaNG.Recruitment.Application.Features.CandidateRecommendations.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ICandidateRecommendationService
    {
        Task<long> CreateAsync(long jobApplicationId, CandidateRecommendationCreateRequest request);

        /// <summary>Most recent recommendation for an application, or null if none exists (pipeline tracker badge, AC4).</summary>
        Task<CandidateRecommendationResponse?> GetLatestByJobApplicationIdAsync(long jobApplicationId);

        /// <summary>Hiring Manager's review queue (AC3).</summary>
        Task<List<CandidateRecommendationPendingListItemResponse>> GetPendingAsync();

        /// <summary>Accept or reject a pending recommendation with comments (AC5).</summary>
        Task ReviewAsync(long candidateRecommendationId, CandidateRecommendationReviewRequest request);
    }
}
