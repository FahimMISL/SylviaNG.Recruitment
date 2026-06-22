using MediatR;
using SylviaNG.Recruitment.Application.Features.Roles.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.Roles.Queries.RoleGetAllPaged
{
    public class RoleGetAllPagedQuery : IRequest<PagedResult<RoleResponse>>
    {
        public PagedRequest Request { get; set; }

        public RoleGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
