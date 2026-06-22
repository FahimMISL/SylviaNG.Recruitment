namespace SylviaNG.Recruitment.Application.Features.Roles.Models
{
    public class PermissionCreateRequest
    {
        public string Module { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
