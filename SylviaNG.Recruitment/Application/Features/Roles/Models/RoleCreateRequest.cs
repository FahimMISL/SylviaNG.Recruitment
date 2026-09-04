namespace SylviaNG.Recruitment.Application.Features.Roles.Models
{
    public class RoleCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public List<PermissionGrantRequest> Permissions { get; set; } = new();
    }
}
