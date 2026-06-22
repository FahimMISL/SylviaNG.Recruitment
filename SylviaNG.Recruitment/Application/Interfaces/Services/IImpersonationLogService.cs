using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IImpersonationLogService
    {
        Task<long> CreateAsync(ImpersonationLogCreateRequest request);
        Task UpdateAsync(long impersonationLogId, ImpersonationLogUpdateRequest request);
        Task DeleteAsync(long impersonationLogId);
        Task<List<ImpersonationLogResponse>> GetAllAsync();
        Task<ImpersonationLogResponse> GetByIdAsync(long impersonationLogId);
        Task<PagedResult<ImpersonationLogResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
