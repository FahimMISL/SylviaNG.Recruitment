using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.IntegrationLogs.Commands.IntegrationLogCreate
{
    public class IntegrationLogCreateHandler : IRequestHandler<IntegrationLogCreateCommand, long>
    {
        private readonly IIntegrationLogService _service;

        public IntegrationLogCreateHandler(IIntegrationLogService service)
        {
            _service = service;
        }

        public async Task<long> Handle(IntegrationLogCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
