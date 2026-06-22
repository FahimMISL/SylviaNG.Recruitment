namespace SylviaNG.Recruitment.Application.Features.Roles.Models
{
    public class RolePermissionAssignRequest
    {
        public List<long> PermissionIds { get; set; } = new();
    }

    public class UserRoleAssignRequest
    {
        public long RoleId { get; set; }
        public long? ScopeBusinessUnitId { get; set; }
        public long? ScopeDepartmentId { get; set; }
    }
}
