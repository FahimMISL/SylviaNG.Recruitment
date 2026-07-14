using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class HiringPipelineRepository : Repository<HiringPipeline>, IHiringPipelineRepository
    {
        public HiringPipelineRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(p => p.Name == name && (!excludeId.HasValue || p.HiringPipelineId != excludeId.Value));
        }

        public async Task<HiringPipeline?> GetByIdWithStagesAsync(long hiringPipelineId)
        {
            return await _dbSet
                .Include(p => p.Stages.OrderBy(s => s.DisplayOrder))
                    .ThenInclude(s => s.Interviewers)
                .Include(p => p.JobPostings)
                .FirstOrDefaultAsync(p => p.HiringPipelineId == hiringPipelineId);
        }

        public async Task<List<HiringPipeline>> GetAllWithStagesAsync()
        {
            return await _dbSet
                .Include(p => p.Stages.OrderBy(s => s.DisplayOrder))
                    .ThenInclude(s => s.Interviewers)
                .Include(p => p.JobPostings)
                .ToListAsync();
        }

        public async Task<List<HiringPipeline>> GetActiveAsync()
        {
            return await _dbSet
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<HashSet<long>> GetExistingEmployeeIdsAsync(IEnumerable<long> employeeIds)
        {
            var ids = employeeIds.ToList();
            var existing = await _dbContext.Employees
                .Where(e => ids.Contains(e.EmployeeId))
                .Select(e => e.EmployeeId)
                .ToListAsync();
            return existing.ToHashSet();
        }

        public async Task<int> CountActiveAsync()
        {
            return await _dbSet.CountAsync(p => p.IsActive);
        }
    }
}
