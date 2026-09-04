using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(d => d.Name == name && (!excludeId.HasValue || d.DepartmentId != excludeId.Value));
        }

        public async Task<List<Department>> GetAllOrderedAsync()
        {
            return await _dbSet.OrderBy(d => d.Name).ToListAsync();
        }

        // Multi-tenant: IgnoreQueryFilters is deliberate here, not a leak - a shared/global
        // department (CompanyId == null) can be referenced by ANY company's JobPostings, so a
        // caller-company-scoped count would under-report usage and let an Admin delete a
        // department another company still depends on. Company-owned custom departments are only
        // ever referenced by their own company's postings anyway (enforced at creation), so this
        // is a no-op difference for those - one code path covers both correctly.
        public async Task<int> CountUsageAsync(long departmentId)
        {
            return await _dbContext.JobPostings.IgnoreQueryFilters().CountAsync(j => j.DepartmentId == departmentId);
        }
    }
}
