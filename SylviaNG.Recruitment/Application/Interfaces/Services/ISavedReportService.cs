using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.SavedReports.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ISavedReportService
    {
        Task<long> CreateAsync(SavedReportCreateRequest request);
        Task UpdateAsync(long savedReportId, SavedReportUpdateRequest request);
        Task DeleteAsync(long savedReportId);
        Task<List<SavedReportResponse>> GetAllAsync();
        Task<SavedReportResponse> GetByIdAsync(long savedReportId);
        Task<PagedResult<SavedReportResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
