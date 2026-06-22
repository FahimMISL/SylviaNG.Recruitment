using MediatR;

namespace SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Commands.IntegrationConfigDelete
{
    public class IntegrationConfigDeleteCommand : IRequest<Unit>
    {
        public long IntegrationConfigId { get; set; }

        public IntegrationConfigDeleteCommand(long integrationConfigId)
        {
            IntegrationConfigId = integrationConfigId;
        }
    }
}
