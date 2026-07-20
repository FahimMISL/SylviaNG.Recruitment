using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IAutoShortlistRunRepository : IRepository<AutoShortlistRun>
    {
        /// <summary>Most recent run for a vacancy, with Results included, or null if none exists yet.</summary>
        Task<AutoShortlistRun?> GetLatestByJobPostingIdAsync(long jobPostingId);

        Task<AutoShortlistRun?> GetByIdWithResultsAsync(long autoShortlistRunId);

        Task<AutoShortlistResult?> GetResultByIdAsync(long autoShortlistResultId);

        void UpdateResult(AutoShortlistResult result);

        /// <summary>JobApplicationId -> Score from the latest run of a vacancy, feeding
        /// ShortlistFilterEvaluationService's MinScreeningScore criterion.</summary>
        Task<Dictionary<long, int>> GetLatestScoresByJobPostingIdAsync(long jobPostingId);
    }
}
