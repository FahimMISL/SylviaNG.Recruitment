using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<User?> GetByKeycloakIdAsync(string keycloakUserId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.KeycloakUserId == keycloakUserId);
        }

        public async Task<bool> ExistsByKeycloakIdAsync(string keycloakUserId, long? excludeId = null)
        {
            return await _dbSet
                .AnyAsync(u => u.KeycloakUserId == keycloakUserId && (!excludeId.HasValue || u.UserId != excludeId.Value));
        }

        public async Task<User?> GetByIdWithRolesAsync(long userId)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<List<User>> GetUsersByRoleNameAsync(string roleName)
        {
            return await _dbSet
                .Where(u => u.IsActive && u.UserRoles.Any(ur => ur.IsActive && ur.Role.Name == roleName))
                .ToListAsync();
        }

        public async Task<List<User>> GetAllActiveUsersAsync()
        {
            return await _dbSet
                .Where(u => u.IsActive && !string.IsNullOrEmpty(u.KeycloakUserId))
                .ToListAsync();
        }
    }
}
