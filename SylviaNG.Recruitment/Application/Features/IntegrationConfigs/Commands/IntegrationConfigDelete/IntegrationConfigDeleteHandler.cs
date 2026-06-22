using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Commands.IntegrationConfigDelete
{
    public class IntegrationConfigDeleteHandler : IRequestHandler<IntegrationConfigDeleteCommand, Unit>
    {
        private readonly IIntegrationConfigService _service;

        public IntegrationConfigDeleteHandler(IIntegrationConfigService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(IntegrationConfigDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.IntegrationConfigId);
            return Unit.Value;
        }
    }
}
