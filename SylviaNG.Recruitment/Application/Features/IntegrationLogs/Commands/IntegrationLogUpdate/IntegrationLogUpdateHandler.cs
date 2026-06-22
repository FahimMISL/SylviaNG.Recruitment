using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.IntegrationLogs.Commands.IntegrationLogUpdate
{
    public class IntegrationLogUpdateHandler : IRequestHandler<IntegrationLogUpdateCommand, Unit>
    {
        private readonly IIntegrationLogService _service;

        public IntegrationLogUpdateHandler(IIntegrationLogService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(IntegrationLogUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.IntegrationLogId, command.Request);
            return Unit.Value;
        }
    }
}
