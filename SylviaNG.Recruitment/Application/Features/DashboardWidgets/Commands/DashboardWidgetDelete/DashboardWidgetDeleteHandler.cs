using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DashboardWidgets.Commands.DashboardWidgetDelete
{
    public class DashboardWidgetDeleteHandler : IRequestHandler<DashboardWidgetDeleteCommand, Unit>
    {
        private readonly IDashboardWidgetService _service;

        public DashboardWidgetDeleteHandler(IDashboardWidgetService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(DashboardWidgetDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.DashboardWidgetId);
            return Unit.Value;
        }
    }
}
