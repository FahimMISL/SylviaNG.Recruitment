using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Commands.IntegrationConfigCreate
{
    public class IntegrationConfigCreateHandler : IRequestHandler<IntegrationConfigCreateCommand, long>
    {
        private readonly IIntegrationConfigService _service;

        public IntegrationConfigCreateHandler(IIntegrationConfigService service)
        {
            _service = service;
        }

        public async Task<long> Handle(IntegrationConfigCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
