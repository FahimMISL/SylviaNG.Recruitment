using MediatR;
using SylviaNG.Recruitment.Application.Features.Roles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Roles.Queries.RoleGetAll
{
    public class RoleGetAllHandler : IRequestHandler<RoleGetAllQuery, List<RoleResponse>>
    {
        private readonly IRoleService _roleService;

        public RoleGetAllHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<List<RoleResponse>> Handle(RoleGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _roleService.GetAllAsync();
        }
    }
}
