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
                Description = request.Description,
                IsSystemRole = request.IsSystemRole,
                IsActive = true
            };
        }

        public static void ApplyUpdate(this Role entity, RoleUpdateRequest request)
        {
            if (request.Name != null) entity.Name = request.Name;
            if (request.Description != null) entity.Description = request.Description;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static RoleResponse ToResponse(this Role entity)
        {
            return new RoleResponse
            {
                RoleId = entity.RoleId,
                Name = entity.Name,
                Description = entity.Description,
                IsSystemRole = entity.IsSystemRole,
                IsActive = entity.IsActive,
                Permissions = entity.RolePermissions?
                    .Where(rp => rp.Permission != null)
                    .Select(rp => rp.Permission!.ToResponse())
                    .ToList() ?? new()
            };
        }

        public static Permission ToEntity(this PermissionCreateRequest request)
        {
            return new Permission
            {
                Module = request.Module,
                Action = request.Action,
                Description = request.Description,
                IsActive = true
            };
        }

        public static PermissionResponse ToResponse(this Permission entity)
        {
            return new PermissionResponse
            {
                PermissionId = entity.PermissionId,
                Module = entity.Module,
                Action = entity.Action,
                Description = entity.Description,
                IsActive = entity.IsActive
            };
        }
    }
}
