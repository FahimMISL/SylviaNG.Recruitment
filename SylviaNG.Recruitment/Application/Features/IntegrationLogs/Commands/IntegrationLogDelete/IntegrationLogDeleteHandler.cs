using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.IntegrationLogs.Commands.IntegrationLogDelete
{
    public class IntegrationLogDeleteHandler : IRequestHandler<IntegrationLogDeleteCommand, Unit>
    {
        private readonly IIntegrationLogService _service;

        public IntegrationLogDeleteHandler(IIntegrationLogService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(IntegrationLogDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.IntegrationLogId);
            return Unit.Value;
        }
    }
}
