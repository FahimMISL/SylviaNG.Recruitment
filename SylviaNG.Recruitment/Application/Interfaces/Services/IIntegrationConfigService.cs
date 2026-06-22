using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IIntegrationConfigService
    {
        Task<long> CreateAsync(IntegrationConfigCreateRequest request);
        Task UpdateAsync(long integrationConfigId, IntegrationConfigUpdateRequest request);
        Task DeleteAsync(long integrationConfigId);
        Task<List<IntegrationConfigResponse>> GetAllAsync();
        Task<IntegrationConfigResponse> GetByIdAsync(long integrationConfigId);
        Task<PagedResult<IntegrationConfigResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
