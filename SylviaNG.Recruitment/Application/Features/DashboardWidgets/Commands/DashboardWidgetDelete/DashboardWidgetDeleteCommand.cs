using MediatR;

namespace SylviaNG.Recruitment.Application.Features.DashboardWidgets.Commands.DashboardWidgetDelete
{
    public class DashboardWidgetDeleteCommand : IRequest<Unit>
    {
        public long DashboardWidgetId { get; set; }

        public DashboardWidgetDeleteCommand(long dashboardWidgetId)
        {
            DashboardWidgetId = dashboardWidgetId;
        }
    }
}
