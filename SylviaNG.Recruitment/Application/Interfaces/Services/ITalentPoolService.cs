using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ITalentPoolService
    {
        Task<long> CreateAsync(TalentPoolCreateRequest request);
        Task UpdateAsync(long talentPoolId, TalentPoolUpdateRequest request);
        Task DeleteAsync(long talentPoolId);
        Task<List<TalentPoolResponse>> GetAllAsync();
        Task<TalentPoolResponse> GetByIdAsync(long talentPoolId);
        Task<PagedResult<TalentPoolResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
