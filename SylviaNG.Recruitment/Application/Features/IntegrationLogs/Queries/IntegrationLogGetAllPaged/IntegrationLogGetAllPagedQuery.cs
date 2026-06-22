using MediatR;
using SylviaNG.Recruitment.Application.Features.IntegrationLogs.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.IntegrationLogs.Queries.IntegrationLogGetAllPaged
{
    public class IntegrationLogGetAllPagedQuery : IRequest<PagedResult<IntegrationLogResponse>>
    {
        public PagedRequest Request { get; set; }

        public IntegrationLogGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
