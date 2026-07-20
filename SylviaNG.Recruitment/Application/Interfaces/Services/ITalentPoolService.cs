using SylviaNG.Recruitment.Application.Features.TalentPools.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ITalentPoolService
    {
        Task<long> CreateAsync(TalentPoolCreateRequest request);
        Task UpdateAsync(long talentPoolId, TalentPoolUpdateRequest request);
        Task DeleteAsync(long talentPoolId);
        Task<List<TalentPoolResponse>> GetAllAsync(long? jobPostingId = null);
        Task<List<TalentPoolLookupResponse>> GetLookupAsync();
        Task<TalentPoolDetailResponse> GetByIdAsync(long talentPoolId);

        Task<TalentPoolCandidateAddResponse> AddCandidatesAsync(long talentPoolId, TalentPoolCandidateAddRequest request);
        Task RemoveCandidateAsync(long talentPoolId, long candidateProfileId);

        /// <summary>
        /// Creates a fresh application (Source=Admin, reusing the candidate's most recent resume
        /// on file) for each pool candidate against the given vacancy and moves it straight to
        /// Shortlisted (US-039 AC4).
        /// </summary>
        Task<TalentPoolFastTrackResponse> FastTrackAsync(TalentPoolFastTrackRequest request);
    }
}
