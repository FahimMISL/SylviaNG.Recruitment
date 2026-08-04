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

        public async Task<int> CountUsageAsync(long departmentId)
        {
            return await _dbContext.JobPostings.CountAsync(j => j.DepartmentId == departmentId);
        }
    }
}
