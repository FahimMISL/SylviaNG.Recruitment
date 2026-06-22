using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DashboardWidgets.Commands.DashboardWidgetCreate
{
    public class DashboardWidgetCreateHandler : IRequestHandler<DashboardWidgetCreateCommand, long>
    {
        private readonly IDashboardWidgetService _service;

        public DashboardWidgetCreateHandler(IDashboardWidgetService service)
        {
            _service = service;
        }

        public async Task<long> Handle(DashboardWidgetCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
