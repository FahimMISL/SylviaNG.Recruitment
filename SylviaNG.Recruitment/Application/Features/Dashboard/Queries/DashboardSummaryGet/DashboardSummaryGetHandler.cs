using MediatR;
using SylviaNG.Recruitment.Application.Features.Dashboard.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Dashboard.Queries.DashboardSummaryGet
{
    public class DashboardSummaryGetHandler : IRequestHandler<DashboardSummaryGetQuery, DashboardSummaryResponse>
    {
        private readonly IDashboardService _dashboardService;

        public DashboardSummaryGetHandler(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<DashboardSummaryResponse> Handle(DashboardSummaryGetQuery query, CancellationToken cancellationToken)
        {
            return await _dashboardService.GetSummaryAsync();
        }
    }
}
