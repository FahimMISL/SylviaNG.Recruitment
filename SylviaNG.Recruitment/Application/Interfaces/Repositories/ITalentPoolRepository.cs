using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ITalentPoolRepository : IRepository<TalentPool>
    {
        Task<bool> ExistsByNameAsync(string name);

        /// <summary>Pool list with candidate counts (US-039 AC1 pool list page).</summary>
        Task<List<TalentPool>> GetAllWithCandidateCountAsync();

        /// <summary>
        /// Pool detail with each member's full candidate-profile snapshot (US-039 AC3), including
        /// the sections CandidateProfileMapper.ToSummaryResponse needs for completeness %.
        /// </summary>
        Task<TalentPool?> GetByIdWithCandidatesAsync(long talentPoolId);
    }
}
