using MediatR;
using SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Models;

namespace SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Commands.IntegrationConfigUpdate
{
    public class IntegrationConfigUpdateCommand : IRequest<Unit>
    {
        public long IntegrationConfigId { get; set; }
        public IntegrationConfigUpdateRequest Request { get; set; }

        public IntegrationConfigUpdateCommand(long integrationConfigId, IntegrationConfigUpdateRequest request)
        {
            IntegrationConfigId = integrationConfigId;
            Request = request;
        }
    }
}
