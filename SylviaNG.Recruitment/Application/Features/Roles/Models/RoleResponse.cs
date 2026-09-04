namespace SylviaNG.Recruitment.Application.Features.Roles.Models
{
    public class RoleResponse
    {
        public long RoleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsSystemRole { get; set; }
        public List<PermissionGrantRequest> Permissions { get; set; } = new();
    }
}
