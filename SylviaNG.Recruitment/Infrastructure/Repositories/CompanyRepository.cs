using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        public CompanyRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(c => c.Name == name && (!excludeId.HasValue || c.CompanyId != excludeId.Value));
        }

        public async Task<List<Company>> GetAllOrderedAsync()
        {
            return await _dbSet.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<(int JobPostingCount, int UserAccountCount)> GetStatsAsync(long companyId)
        {
            var jobPostingCount = await _dbContext.JobPostings.IgnoreQueryFilters().CountAsync(j => j.CompanyId == companyId);
            var userAccountCount = await _dbContext.UserAccounts.IgnoreQueryFilters().CountAsync(u => u.CompanyId == companyId);
            return (jobPostingCount, userAccountCount);
        }
    }
}
