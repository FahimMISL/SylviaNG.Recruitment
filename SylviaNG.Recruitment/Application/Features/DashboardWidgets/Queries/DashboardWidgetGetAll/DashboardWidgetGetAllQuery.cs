using MediatR;
using SylviaNG.Recruitment.Application.Features.DashboardWidgets.Models;

namespace SylviaNG.Recruitment.Application.Features.DashboardWidgets.Queries.DashboardWidgetGetAll
{
    public class DashboardWidgetGetAllQuery : IRequest<List<DashboardWidgetResponse>>
    {
    }
}
