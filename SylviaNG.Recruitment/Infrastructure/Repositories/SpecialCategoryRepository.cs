using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class SpecialCategoryRepository : Repository<SpecialCategory>, ISpecialCategoryRepository
    {
        public SpecialCategoryRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(s => s.Name == name && (!excludeId.HasValue || s.SpecialCategoryId != excludeId.Value));
        }

        public async Task<List<SpecialCategory>> GetAllOrderedAsync()
        {
            return await _dbSet.OrderBy(s => s.Name).ToListAsync();
        }

        public async Task<int> CountUsageAsync(long specialCategoryId)
        {
            var applicationUsage = await _dbContext.JobApplications.CountAsync(a => a.SpecialCategoryId == specialCategoryId);
            var ruleUsage = await _dbContext.WaiverRules.CountAsync(w => w.SpecialCategoryId == specialCategoryId);
            return applicationUsage + ruleUsage;
        }
    }
}
