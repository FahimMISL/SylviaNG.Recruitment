using MediatR;
using SylviaNG.Recruitment.Application.Features.DashboardWidgets.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DashboardWidgets.Queries.DashboardWidgetGetAll
{
    public class DashboardWidgetGetAllHandler : IRequestHandler<DashboardWidgetGetAllQuery, List<DashboardWidgetResponse>>
    {
        private readonly IDashboardWidgetService _service;

        public DashboardWidgetGetAllHandler(IDashboardWidgetService service)
        {
            _service = service;
        }

        public async Task<List<DashboardWidgetResponse>> Handle(DashboardWidgetGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
