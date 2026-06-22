using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IPermissionRepository : IRepository<Permission>
    {
        Task<bool> ExistsByModuleAndActionAsync(string module, string action, long? excludeId = null);
    }
}
