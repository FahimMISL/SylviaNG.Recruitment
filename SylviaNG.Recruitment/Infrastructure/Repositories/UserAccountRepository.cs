using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class UserAccountRepository : Repository<UserAccount>, IUserAccountRepository
    {
        public UserAccountRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByEmailAsync(string email, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(u => u.Email == email && (!excludeId.HasValue || u.UserAccountId != excludeId.Value));
        }

        public async Task<bool> ExistsAnyWithRoleAsync(string roleName)
        {
            return await _dbSet.AnyAsync(u => u.RoleAssignments.Any(a => a.Role.Name == roleName));
        }

        public async Task<List<string>> GetActiveEmailsByRoleAsync(string roleName)
        {
            return await _dbSet
                .Where(u => u.IsActive && u.RoleAssignments.Any(a => a.Role.Name == roleName) && !string.IsNullOrWhiteSpace(u.Email))
                .Select(u => u.Email)
                .ToListAsync();
        }

        public async Task<UserAccount?> GetByIdWithRolesAsync(long userAccountId)
        {
            return await _dbSet
                .Include(u => u.RoleAssignments).ThenInclude(a => a.Role).ThenInclude(r => r.Permissions)
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.UserAccountId == userAccountId);
        }

        public async Task<List<UserAccount>> GetAllWithRolesAsync()
        {
            return await _dbSet
                .Include(u => u.RoleAssignments).ThenInclude(a => a.Role).ThenInclude(r => r.Permissions)
                .Include(u => u.Company)
                .OrderByDescending(u => u.CreatedAt)
                .ThenByDescending(u => u.UserAccountId)
                .ToListAsync();
        }

        public async Task<UserAccount?> GetByKeycloakUserIdWithRolesAsync(string keycloakUserId)
        {
            return await _dbSet
                .Include(u => u.RoleAssignments).ThenInclude(a => a.Role).ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(u => u.KeycloakUserId == keycloakUserId);
        }

        public async Task<long?> GetIdByKeycloakUserIdAsync(string keycloakUserId)
        {
            return await _dbSet
                .Where(u => u.KeycloakUserId == keycloakUserId)
                .Select(u => (long?)u.UserAccountId)
                .FirstOrDefaultAsync();
        }
    }
}
