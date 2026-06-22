using MediatR;
using SylviaNG.Recruitment.Application.Features.DashboardWidgets.Models;

namespace SylviaNG.Recruitment.Application.Features.DashboardWidgets.Commands.DashboardWidgetCreate
{
    public class DashboardWidgetCreateCommand : IRequest<long>
    {
        public DashboardWidgetCreateRequest Request { get; set; }

        public DashboardWidgetCreateCommand(DashboardWidgetCreateRequest request)
        {
            Request = request;
        }
    }
}
