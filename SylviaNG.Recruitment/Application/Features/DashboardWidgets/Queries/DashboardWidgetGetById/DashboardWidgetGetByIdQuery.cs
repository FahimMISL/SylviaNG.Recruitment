using MediatR;
using SylviaNG.Recruitment.Application.Features.DashboardWidgets.Models;

namespace SylviaNG.Recruitment.Application.Features.DashboardWidgets.Queries.DashboardWidgetGetById
{
    public class DashboardWidgetGetByIdQuery : IRequest<DashboardWidgetResponse>
    {
        public long DashboardWidgetId { get; set; }

        public DashboardWidgetGetByIdQuery(long dashboardWidgetId)
        {
            DashboardWidgetId = dashboardWidgetId;
        }
    }
}
