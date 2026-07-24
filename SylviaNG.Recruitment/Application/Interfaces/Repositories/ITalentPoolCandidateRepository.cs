using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ITalentPoolCandidateRepository : IRepository<TalentPoolCandidate>
    {
        Task<TalentPoolCandidate?> GetByPoolAndCandidateAsync(long talentPoolId, long candidateProfileId);

        /// <summary>Which of the given candidate ids are already in this pool, to skip on bulk-add (US-039 AC1).</summary>
        Task<List<long>> GetExistingCandidateIdsAsync(long talentPoolId, IEnumerable<long> candidateProfileIds);

        /// <summary>Pools a candidate belongs to, with pool name, for the profile badge (US-039 AC2).</summary>
        Task<List<TalentPoolCandidate>> GetAllByCandidateProfileIdAsync(long candidateProfileId);
    }
}
