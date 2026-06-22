using MediatR;
using SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Queries.IntegrationConfigGetAllPaged
{
    public class IntegrationConfigGetAllPagedQuery : IRequest<PagedResult<IntegrationConfigResponse>>
    {
        public PagedRequest Request { get; set; }

        public IntegrationConfigGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
