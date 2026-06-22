using SylviaNG.Recruitment.Application.Features.Users.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class UserMapper
    {
        public static User ToEntity(this UserCreateRequest request)
        {
            return new User
            {
                KeycloakUserId = request.KeycloakUserId,
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                EmployeeId = request.EmployeeId,
                DepartmentId = request.DepartmentId,
                SiteId = request.SiteId,
                IsActive = true
            };
        }

        public static void ApplyUpdate(this User entity, UserUpdateRequest request)
        {
            if (request.FullName != null) entity.FullName = request.FullName;
            if (request.Email != null) entity.Email = request.Email;
            if (request.Phone != null) entity.Phone = request.Phone;
            if (request.EmployeeId.HasValue) entity.EmployeeId = request.EmployeeId;
            if (request.DepartmentId.HasValue) entity.DepartmentId = request.DepartmentId;
            if (request.SiteId.HasValue) entity.SiteId = request.SiteId;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static UserResponse ToResponse(this User entity)
        {
            return new UserResponse
            {
                UserId = entity.UserId,
                KeycloakUserId = entity.KeycloakUserId,
                FullName = entity.FullName,
                Email = entity.Email,
                Phone = entity.Phone,
                EmployeeId = entity.EmployeeId,
                DepartmentId = entity.DepartmentId,
                SiteId = entity.SiteId,
                IsActive = entity.IsActive,
                Roles = entity.UserRoles?.Select(ur => ur.Role?.Name ?? "").Where(n => n != "").ToList() ?? new()
            };
        }
    }
}
