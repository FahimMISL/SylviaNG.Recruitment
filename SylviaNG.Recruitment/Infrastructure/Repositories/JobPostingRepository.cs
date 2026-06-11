using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class JobPostingRepository : Repository<JobPosting>, IJobPostingRepository
    {
        public JobPostingRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<JobPosting?> GetByTitleAndSiteIdAsync(string title, long siteId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(j => j.Title == title && j.SiteId == siteId);
        }

        public async Task<bool> ExistsByTitleAndSiteIdAsync(string title, long siteId, long? excludeId = null)
        {
            return await _dbSet
                .AnyAsync(j => j.Title == title && j.SiteId == siteId && (!excludeId.HasValue || j.JobPostingId != excludeId.Value));
        }

        public async Task<PagedResult<JobPosting>> GetPaginatedAsync(PagedRequest request)
        {
            var query = _dbSet
                .Include(j => j.Applications)
                .AsQueryable();

            return await query.ToPaginatedResultAsync(request);
        }

        public async Task<List<JobPosting>> GetActiveBySiteIdAsync(long siteId)
        {
            return await _dbSet
                .Where(j => j.SiteId == siteId && j.IsActive)
                .ToListAsync();
        }
    }
}
