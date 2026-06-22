using MediatR;
using SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Models;

namespace SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Queries.IntegrationConfigGetById
{
    public class IntegrationConfigGetByIdQuery : IRequest<IntegrationConfigResponse>
    {
        public long IntegrationConfigId { get; set; }

        public IntegrationConfigGetByIdQuery(long integrationConfigId)
        {
            IntegrationConfigId = integrationConfigId;
        }
    }
}
