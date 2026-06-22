using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ITalentPoolCandidateService
    {
        Task<long> CreateAsync(TalentPoolCandidateCreateRequest request);
        Task UpdateAsync(long talentPoolCandidateId, TalentPoolCandidateUpdateRequest request);
        Task DeleteAsync(long talentPoolCandidateId);
        Task<List<TalentPoolCandidateResponse>> GetAllAsync();
        Task<TalentPoolCandidateResponse> GetByIdAsync(long talentPoolCandidateId);
        Task<PagedResult<TalentPoolCandidateResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
