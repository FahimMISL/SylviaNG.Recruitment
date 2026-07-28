using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class ReferralSourceRepository : Repository<ReferralSource>, IReferralSourceRepository
    {
        public ReferralSourceRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(r => r.Name == name && (!excludeId.HasValue || r.ReferralSourceId != excludeId.Value));
        }

        public async Task<List<ReferralSource>> GetAllOrderedAsync()
        {
            return await _dbSet.OrderBy(r => r.Name).ToListAsync();
        }

        public async Task<int> CountUsageAsync(long referralSourceId)
        {
            var applicationUsage = await _dbContext.JobApplications.CountAsync(a => a.ReferralSourceId == referralSourceId);
            var ruleUsage = await _dbContext.WaiverRules.CountAsync(w => w.ReferralSourceId == referralSourceId);
            return applicationUsage + ruleUsage;
        }
    }
}
