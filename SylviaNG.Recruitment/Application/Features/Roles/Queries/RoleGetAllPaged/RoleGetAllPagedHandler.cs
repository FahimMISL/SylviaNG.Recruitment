using MediatR;
using SylviaNG.Recruitment.Application.Features.Roles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.Roles.Queries.RoleGetAllPaged
{
    public class RoleGetAllPagedHandler : IRequestHandler<RoleGetAllPagedQuery, PagedResult<RoleResponse>>
    {
        private readonly IRoleService _roleService;

        public RoleGetAllPagedHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<PagedResult<RoleResponse>> Handle(RoleGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _roleService.GetPaginatedAsync(query.Request);
        }
    }
}
