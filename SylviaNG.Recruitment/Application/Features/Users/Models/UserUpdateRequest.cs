namespace SylviaNG.Recruitment.Application.Features.Users.Models
{
    public class UserUpdateRequest
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public long? EmployeeId { get; set; }
        public long? DepartmentId { get; set; }
        public long? SiteId { get; set; }
        public bool? IsActive { get; set; }
    }
}
