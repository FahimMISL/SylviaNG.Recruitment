namespace SylviaNG.Recruitment.Application.Features.UserAccounts.Models
{
    public class UserAccountResponse
    {
        public long UserAccountId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<long> RoleIds { get; set; } = new();
        public List<string> RoleNames { get; set; } = new();
        public long? CompanyId { get; set; }
        public string? CompanyName { get; set; }
    }
}
