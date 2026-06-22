using MediatR;
using SylviaNG.Recruitment.Application.Features.DashboardWidgets.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DashboardWidgets.Queries.DashboardWidgetGetById
{
    public class DashboardWidgetGetByIdHandler : IRequestHandler<DashboardWidgetGetByIdQuery, DashboardWidgetResponse>
    {
        private readonly IDashboardWidgetService _service;

        public DashboardWidgetGetByIdHandler(IDashboardWidgetService service)
        {
            _service = service;
        }

        public async Task<DashboardWidgetResponse> Handle(DashboardWidgetGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.DashboardWidgetId);
        }
    }
}
