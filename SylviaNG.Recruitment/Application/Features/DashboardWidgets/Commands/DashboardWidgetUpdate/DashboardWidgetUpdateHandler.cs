using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DashboardWidgets.Commands.DashboardWidgetUpdate
{
    public class DashboardWidgetUpdateHandler : IRequestHandler<DashboardWidgetUpdateCommand, Unit>
    {
        private readonly IDashboardWidgetService _service;

        public DashboardWidgetUpdateHandler(IDashboardWidgetService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(DashboardWidgetUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.DashboardWidgetId, command.Request);
            return Unit.Value;
        }
    }
}
