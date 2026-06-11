using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IJobPostingRepository : IRepository<JobPosting>
    {
        Task<JobPosting?> GetByTitleAndSiteIdAsync(string title, long siteId);
        Task<bool> ExistsByTitleAndSiteIdAsync(string title, long siteId, long? excludeId = null);
        Task<PagedResult<JobPosting>> GetPaginatedAsync(PagedRequest request);
        Task<List<JobPosting>> GetActiveBySiteIdAsync(long siteId);
    }
}
