using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Commands.IntegrationConfigUpdate
{
    public class IntegrationConfigUpdateHandler : IRequestHandler<IntegrationConfigUpdateCommand, Unit>
    {
        private readonly IIntegrationConfigService _service;

        public IntegrationConfigUpdateHandler(IIntegrationConfigService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(IntegrationConfigUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.IntegrationConfigId, command.Request);
            return Unit.Value;
        }
    }
}
