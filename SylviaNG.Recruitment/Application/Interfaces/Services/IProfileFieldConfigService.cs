using SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IProfileFieldConfigService
    {
        Task<long> CreateAsync(ProfileFieldConfigCreateRequest request);
        Task UpdateAsync(long id, ProfileFieldConfigUpdateRequest request);
        Task DeleteAsync(long id);
        Task<List<ProfileFieldConfigResponse>> GetAllAsync();
        Task<ProfileFieldConfigResponse> GetByIdAsync(long id);
        Task<PagedResult<ProfileFieldConfigResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
