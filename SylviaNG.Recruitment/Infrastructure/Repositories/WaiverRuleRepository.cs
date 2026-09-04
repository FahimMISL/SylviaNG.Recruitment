using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class WaiverRuleRepository : Repository<WaiverRule>, IWaiverRuleRepository
    {
        public WaiverRuleRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(w => w.Name == name && (!excludeId.HasValue || w.WaiverRuleId != excludeId.Value));
        }

        public async Task<List<WaiverRule>> GetAllOrderedAsync()
        {
            return await _dbSet
                .Include(w => w.SpecialCategory)
                .Include(w => w.ReferralSource)
                .OrderBy(w => w.Priority).ThenBy(w => w.Name)
                .ToListAsync();
        }

        public async Task<List<WaiverRule>> GetActiveOrderedByPriorityAsync()
        {
            return await _dbSet
                .Where(w => w.IsActive)
                .OrderBy(w => w.Priority).ThenBy(w => w.WaiverRuleId)
                .ToListAsync();
        }
    }
}
