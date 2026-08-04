using SylviaNG.Recruitment.Application.Features.UserAccounts.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class UserAccountMapper
    {
        public static UserAccountResponse ToResponse(this UserAccount entity)
        {
            return new UserAccountResponse
            {
                UserAccountId = entity.UserAccountId,
                Email = entity.Email,
                FullName = entity.FullName,
                IsActive = entity.IsActive,
                RoleIds = entity.RoleAssignments.Select(a => a.RoleId).ToList(),
                RoleNames = entity.RoleAssignments.Select(a => a.Role.Name).ToList()
            };
        }
    }
}
