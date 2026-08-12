namespace SylviaNG.Recruitment.Application.Features.UserAccounts.Models
{
    public class UserAccountCreateRequest
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<long> RoleIds { get; set; } = new();
    }
}
