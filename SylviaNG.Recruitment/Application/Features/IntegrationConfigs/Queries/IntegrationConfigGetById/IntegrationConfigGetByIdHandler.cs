using MediatR;
using SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Queries.IntegrationConfigGetById
{
    public class IntegrationConfigGetByIdHandler : IRequestHandler<IntegrationConfigGetByIdQuery, IntegrationConfigResponse>
    {
        private readonly IIntegrationConfigService _service;

        public IntegrationConfigGetByIdHandler(IIntegrationConfigService service)
        {
            _service = service;
        }

        public async Task<IntegrationConfigResponse> Handle(IntegrationConfigGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.IntegrationConfigId);
        }
    }
}
