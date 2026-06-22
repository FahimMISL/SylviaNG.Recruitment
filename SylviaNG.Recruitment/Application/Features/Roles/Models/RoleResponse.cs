namespace SylviaNG.Recruitment.Application.Features.Roles.Models
{
    public class RoleResponse
    {
        public long RoleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSystemRole { get; set; }
        public bool IsActive { get; set; }
        public List<PermissionResponse> Permissions { get; set; } = new();
    }

    public class PermissionResponse
    {
        public long PermissionId { get; set; }
        public string Module { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
