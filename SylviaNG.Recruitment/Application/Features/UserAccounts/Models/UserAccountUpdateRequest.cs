namespace SylviaNG.Recruitment.Application.Features.UserAccounts.Models
{
    public class UserAccountUpdateRequest
    {
        public string FullName { get; set; } = string.Empty;
        public List<long> RoleIds { get; set; } = new();
    }
}
