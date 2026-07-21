using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
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
                .Include(j => j.HiringPipeline)
                .Include(j => j.AssessmentWorkflow)
                .AsQueryable();

            return await query.ToPaginatedResultAsync(request);
        }

        public async Task<List<JobPosting>> GetActiveBySiteIdAsync(long siteId)
        {
            return await _dbSet
                .Where(j => j.SiteId == siteId && j.IsActive)
                .ToListAsync();
        }

        public async Task<PagedResult<JobPosting>> GetPaginatedByCircularTypesAsync(
            PagedRequest request,
            IReadOnlyCollection<CircularTypeEnum> allowedCircularTypes,
            string? location,
            long? departmentId,
            EmploymentTypeEnum? employmentType,
            int? maxExperienceYears)
        {
            var query = ApplyAudienceFilter(_dbSet.AsQueryable(), allowedCircularTypes)
                .Where(j => location == null || (j.Location != null && j.Location.Contains(location)))
                .Where(j => departmentId == null || j.DepartmentId == departmentId)
                .Where(j => employmentType == null || j.EmploymentType == employmentType)
                .Where(j => maxExperienceYears == null || j.MinExperienceYears == null || j.MinExperienceYears <= maxExperienceYears);

            return await query.ToPaginatedResultAsync(request);
        }

        public async Task<JobPosting?> GetOpenByIdAndCircularTypesAsync(long jobPostingId, IReadOnlyCollection<CircularTypeEnum> allowedCircularTypes)
        {
            return await ApplyAudienceFilter(_dbSet.AsQueryable(), allowedCircularTypes)
                .FirstOrDefaultAsync(j => j.JobPostingId == jobPostingId);
        }

        private static IQueryable<JobPosting> ApplyAudienceFilter(IQueryable<JobPosting> query, IReadOnlyCollection<CircularTypeEnum> allowedCircularTypes)
        {
            var now = DateTime.UtcNow;
            return query.Where(j => j.Status == JobStatusEnum.Open && allowedCircularTypes.Contains(j.CircularType) && (j.ClosingDate == null || j.ClosingDate >= now));
        }

        public async Task<int> CountByStatusAsync(JobStatusEnum status)
        {
            return await _dbSet.CountAsync(j => j.Status == status);
        }
    }
}
