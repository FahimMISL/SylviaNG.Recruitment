namespace SylviaNG.Recruitment.Application.Features.Roles.Models
{
    public class RoleUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public List<PermissionGrantRequest> Permissions { get; set; } = new();
    }
}
