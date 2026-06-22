using MediatR;
using SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Queries.IntegrationConfigGetAllPaged
{
    public class IntegrationConfigGetAllPagedHandler : IRequestHandler<IntegrationConfigGetAllPagedQuery, PagedResult<IntegrationConfigResponse>>
    {
        private readonly IIntegrationConfigService _integrationConfigService;

        public IntegrationConfigGetAllPagedHandler(IIntegrationConfigService integrationConfigService)
        {
            _integrationConfigService = integrationConfigService;
        }

        public async Task<PagedResult<IntegrationConfigResponse>> Handle(IntegrationConfigGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _integrationConfigService.GetPaginatedAsync(query.Request);
        }
    }
}
