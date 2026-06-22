using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByKeycloakIdAsync(string keycloakUserId);
        Task<bool> ExistsByKeycloakIdAsync(string keycloakUserId, long? excludeId = null);
        Task<User?> GetByIdWithRolesAsync(long userId);
        Task<List<User>> GetUsersByRoleNameAsync(string roleName);
        Task<List<User>> GetAllActiveUsersAsync();
    }
}
