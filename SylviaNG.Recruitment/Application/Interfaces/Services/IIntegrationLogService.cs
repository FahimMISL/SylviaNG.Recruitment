using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.IntegrationLogs.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IIntegrationLogService
    {
        Task<long> CreateAsync(IntegrationLogCreateRequest request);
        Task UpdateAsync(long integrationLogId, IntegrationLogUpdateRequest request);
        Task DeleteAsync(long integrationLogId);
        Task<List<IntegrationLogResponse>> GetAllAsync();
        Task<IntegrationLogResponse> GetByIdAsync(long integrationLogId);
        Task<PagedResult<IntegrationLogResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
