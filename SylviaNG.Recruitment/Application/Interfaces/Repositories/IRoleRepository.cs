using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task<Role?> GetByNameAsync(string name);
        Task<Role?> GetByIdWithPermissionsAsync(long roleId);
        Task<List<Role>> GetAllWithPermissionsAsync();
        Task<int> CountAssignedUsersAsync(long roleId);
    }
}
