using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ITalentPoolRepository : IRepository<TalentPool>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);

        /// <summary>Pool list with candidate counts (US-039 AC1 pool list page), optionally filtered to pools linked to a specific job vacancy.</summary>
        Task<List<TalentPool>> GetAllWithCandidateCountAsync(long? jobPostingId = null);

        /// <summary>
        /// Pool detail with each member's full candidate-profile snapshot (US-039 AC3), including
        /// the sections CandidateProfileMapper.ToSummaryResponse needs for completeness %.
        /// </summary>
        Task<TalentPool?> GetByIdWithCandidatesAsync(long talentPoolId);
    }
}
