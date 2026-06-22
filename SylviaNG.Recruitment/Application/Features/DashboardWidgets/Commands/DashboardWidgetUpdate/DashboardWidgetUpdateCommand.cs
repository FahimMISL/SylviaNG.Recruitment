using MediatR;
using SylviaNG.Recruitment.Application.Features.DashboardWidgets.Models;

namespace SylviaNG.Recruitment.Application.Features.DashboardWidgets.Commands.DashboardWidgetUpdate
{
    public class DashboardWidgetUpdateCommand : IRequest<Unit>
    {
        public long DashboardWidgetId { get; set; }
        public DashboardWidgetUpdateRequest Request { get; set; }

        public DashboardWidgetUpdateCommand(long dashboardWidgetId, DashboardWidgetUpdateRequest request)
        {
            DashboardWidgetId = dashboardWidgetId;
            Request = request;
        }
    }
}
