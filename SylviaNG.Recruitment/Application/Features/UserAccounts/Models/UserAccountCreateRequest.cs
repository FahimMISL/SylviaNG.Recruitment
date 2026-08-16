namespace SylviaNG.Recruitment.Application.Features.UserAccounts.Models
{
    public class UserAccountCreateRequest
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<long> RoleIds { get; set; } = new();

        // Multi-tenant: required for Admin/HR invites (validated in UserAccountCreateValidator);
        // left null when RoleIds resolves to SuperAdmin only, which has no company.
        public long? CompanyId { get; set; }
    }
}
