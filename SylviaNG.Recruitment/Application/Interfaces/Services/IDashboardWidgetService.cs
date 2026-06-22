using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.DashboardWidgets.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IDashboardWidgetService
    {
        Task<long> CreateAsync(DashboardWidgetCreateRequest request);
        Task UpdateAsync(long dashboardWidgetId, DashboardWidgetUpdateRequest request);
        Task DeleteAsync(long dashboardWidgetId);
        Task<List<DashboardWidgetResponse>> GetAllAsync();
        Task<DashboardWidgetResponse> GetByIdAsync(long dashboardWidgetId);
        Task<PagedResult<DashboardWidgetResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
