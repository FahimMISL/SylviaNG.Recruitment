using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IUserAccountRepository : IRepository<UserAccount>
    {
        Task<bool> ExistsByEmailAsync(string email, long? excludeId = null);
        Task<bool> ExistsAnyWithRoleAsync(string roleName);
        /// <summary>Active users holding <paramref name="roleName"/>. Pass <paramref name="companyId"/>
        /// whenever the caller knows the tenant - the ambient ICompanyScoped filter matches every
        /// company when no scope is set (anonymous endpoints, background workers).</summary>
        Task<List<string>> GetActiveEmailsByRoleAsync(string roleName, long? companyId = null);
        Task<UserAccount?> GetByIdWithRolesAsync(long userAccountId);
        Task<List<UserAccount>> GetAllWithRolesAsync();
        Task<UserAccount?> GetByKeycloakUserIdWithRolesAsync(string keycloakUserId);

        /// <summary>Lightweight lookup for resolving the current request's local UserAccountId
        /// (e.g. to stamp JobPosting.CreatedBy) without loading the full role/permission graph.</summary>
        Task<long?> GetIdByKeycloakUserIdAsync(string keycloakUserId);
    }
}
