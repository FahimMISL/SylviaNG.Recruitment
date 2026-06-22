using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class PermissionRepository : Repository<Permission>, IPermissionRepository
    {
        public PermissionRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByModuleAndActionAsync(string module, string action, long? excludeId = null)
        {
            return await _dbSet
                .AnyAsync(p => p.Module == module && p.Action == action && (!excludeId.HasValue || p.PermissionId != excludeId.Value));
        }
    }
}
