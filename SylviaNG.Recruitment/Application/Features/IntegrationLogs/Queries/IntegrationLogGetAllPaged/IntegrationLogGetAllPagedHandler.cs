using MediatR;
using SylviaNG.Recruitment.Application.Features.IntegrationLogs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.IntegrationLogs.Queries.IntegrationLogGetAllPaged
{
    public class IntegrationLogGetAllPagedHandler : IRequestHandler<IntegrationLogGetAllPagedQuery, PagedResult<IntegrationLogResponse>>
    {
        private readonly IIntegrationLogService _integrationLogService;

        public IntegrationLogGetAllPagedHandler(IIntegrationLogService integrationLogService)
        {
            _integrationLogService = integrationLogService;
        }

        public async Task<PagedResult<IntegrationLogResponse>> Handle(IntegrationLogGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _integrationLogService.GetPaginatedAsync(query.Request);
        }
    }
}
