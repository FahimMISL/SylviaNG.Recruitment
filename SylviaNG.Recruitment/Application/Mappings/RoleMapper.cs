using SylviaNG.Recruitment.Application.Features.Roles.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class RoleMapper
    {
        public static Role ToEntity(this RoleCreateRequest request)
        {
            return new Role
            {
                Name = request.Name,
                IsSystemRole = false,
                Permissions = request.Permissions
                    .Select(p => new RolePermission { Module = p.Module, Action = p.Action })
                    .ToList()
            };
        }

        public static RoleResponse ToResponse(this Role entity)
        {
            return new RoleResponse
            {
                RoleId = entity.RoleId,
                Name = entity.Name,
                IsSystemRole = entity.IsSystemRole,
                Permissions = entity.Permissions
                    .Select(p => new PermissionGrantRequest { Module = p.Module, Action = p.Action })
                    .ToList()
            };
        }
    }
}
