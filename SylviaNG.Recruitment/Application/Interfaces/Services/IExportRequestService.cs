using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IExportRequestService
    {
        Task<long> CreateAsync(ExportRequestCreateRequest request);
        Task UpdateAsync(long exportRequestId, ExportRequestUpdateRequest request);
        Task DeleteAsync(long exportRequestId);
        Task<List<ExportRequestResponse>> GetAllAsync();
        Task<ExportRequestResponse> GetByIdAsync(long exportRequestId);
        Task<PagedResult<ExportRequestResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
