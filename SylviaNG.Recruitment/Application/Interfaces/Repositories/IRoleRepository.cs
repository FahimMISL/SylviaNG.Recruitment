using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task<Role?> GetByIdWithPermissionsAsync(long roleId);
    }
}
