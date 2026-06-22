namespace SylviaNG.Recruitment.Application.Features.Users.Models
{
    public class UserResponse
    {
        public long UserId { get; set; }
        public string KeycloakUserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public long? EmployeeId { get; set; }
        public long? DepartmentId { get; set; }
        public long? SiteId { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
