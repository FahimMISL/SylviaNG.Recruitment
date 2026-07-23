using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ICandidateRecommendationRepository : IRepository<CandidateRecommendation>
    {
        /// <summary>Most recent recommendation for an application, for the pipeline tracker badge (AC4).</summary>
        Task<CandidateRecommendation?> GetLatestByJobApplicationIdAsync(long jobApplicationId);

        /// <summary>All Pending recommendations, with JobApplication loaded, for the Hiring Manager's review queue (AC3).</summary>
        Task<List<CandidateRecommendation>> GetPendingWithApplicationAsync();
    }
}
