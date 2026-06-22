using MediatR;
using SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Queries.ImpersonationLogGetAllPaged
{
    public class ImpersonationLogGetAllPagedQuery : IRequest<PagedResult<ImpersonationLogResponse>>
    {
        public PagedRequest Request { get; set; }

        public ImpersonationLogGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
