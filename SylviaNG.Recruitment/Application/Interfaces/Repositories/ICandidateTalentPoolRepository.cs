using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ICandidateTalentPoolRepository : IRepository<CandidateTalentPool>
    {
        /// <summary>Which of these candidate profile IDs are already in the talent pool.</summary>
        Task<HashSet<long>> GetExistingCandidateProfileIdsAsync(IEnumerable<long> candidateProfileIds);

        /// <summary>All talent pool entries with the candidate profile included, newest first.</summary>
        Task<List<CandidateTalentPool>> GetAllWithProfileAsync();

        Task<CandidateTalentPool?> GetByCandidateProfileIdAsync(long candidateProfileId);
    }
}
