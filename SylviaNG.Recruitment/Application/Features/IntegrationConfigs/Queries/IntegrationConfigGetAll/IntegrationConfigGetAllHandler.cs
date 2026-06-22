using MediatR;
using SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Queries.IntegrationConfigGetAll
{
    public class IntegrationConfigGetAllHandler : IRequestHandler<IntegrationConfigGetAllQuery, List<IntegrationConfigResponse>>
    {
        private readonly IIntegrationConfigService _service;

        public IntegrationConfigGetAllHandler(IIntegrationConfigService service)
        {
            _service = service;
        }

        public async Task<List<IntegrationConfigResponse>> Handle(IntegrationConfigGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
