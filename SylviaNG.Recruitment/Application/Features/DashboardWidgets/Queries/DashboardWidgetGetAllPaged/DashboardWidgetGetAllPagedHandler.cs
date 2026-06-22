using MediatR;
using SylviaNG.Recruitment.Application.Features.DashboardWidgets.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.DashboardWidgets.Queries.DashboardWidgetGetAllPaged
{
    public class DashboardWidgetGetAllPagedHandler : IRequestHandler<DashboardWidgetGetAllPagedQuery, PagedResult<DashboardWidgetResponse>>
    {
        private readonly IDashboardWidgetService _dashboardWidgetService;

        public DashboardWidgetGetAllPagedHandler(IDashboardWidgetService dashboardWidgetService)
        {
            _dashboardWidgetService = dashboardWidgetService;
        }

        public async Task<PagedResult<DashboardWidgetResponse>> Handle(DashboardWidgetGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _dashboardWidgetService.GetPaginatedAsync(query.Request);
        }
    }
}
