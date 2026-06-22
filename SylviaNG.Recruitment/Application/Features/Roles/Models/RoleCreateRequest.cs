namespace SylviaNG.Recruitment.Application.Features.Roles.Models
{
    public class RoleCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSystemRole { get; set; }
    }
}
