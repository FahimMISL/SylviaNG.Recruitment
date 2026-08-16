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

        public async Task<bool> ExistsByTitleAsync(string title, long? excludeId = null)
        {
            return await _dbSet
                .AnyAsync(j => j.Title == title && (!excludeId.HasValue || j.JobPostingId != excludeId.Value));
        }

        public async Task<PagedResult<JobPosting>> GetPaginatedAsync(PagedRequest request)
        {
            var statusMatches = GetMatchingEnumValues<JobStatusEnum>(request.SearchTerm);
            var circularTypeMatches = GetMatchingEnumValues<CircularTypeEnum>(request.SearchTerm);

            var query = _dbSet
                .Include(j => j.Applications)
                .Include(j => j.HiringPipeline)
                .Include(j => j.Department)
                .OrderByDescending(j => j.CreatedAt)
                .ThenByDescending(j => j.JobPostingId)
                .AsQueryable();

            if (request.PostingDateFrom.HasValue)
            {
                query = query.Where(j => j.PostingDate >= request.PostingDateFrom.Value.Date);
            }

            if (request.PostingDateTo.HasValue)
            {
                var postingDateToExclusive = request.PostingDateTo.Value.Date.AddDays(1);
                query = query.Where(j => j.PostingDate < postingDateToExclusive);
            }

            if (request.ClosingDateFrom.HasValue)
            {
                query = query.Where(j => j.ClosingDate >= request.ClosingDateFrom.Value.Date);
            }

            if (request.ClosingDateTo.HasValue)
            {
                var closingDateToExclusive = request.ClosingDateTo.Value.Date.AddDays(1);
                query = query.Where(j => j.ClosingDate < closingDateToExclusive);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var normalizedSearchTerm = request.SearchTerm.Trim().ToLowerInvariant();

                query = query.Where(j =>
                    j.Title.ToLower().Contains(normalizedSearchTerm) ||
                    j.JobPostingCode.ToLower().Contains(normalizedSearchTerm) ||
                    (j.Location != null && j.Location.ToLower().Contains(normalizedSearchTerm)) ||
                    statusMatches.Contains(j.Status) ||
                    circularTypeMatches.Contains(j.CircularType));
            }

            return await query.ToPaginatedResultAsync(request);
        }

        private static TEnum[] GetMatchingEnumValues<TEnum>(string? searchTerm) where TEnum : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Array.Empty<TEnum>();
            }

            var normalizedSearchTerm = NormalizeEnumText(searchTerm);
            return Enum.GetValues<TEnum>()
                .Where(value => NormalizeEnumText(value.ToString()).Contains(normalizedSearchTerm))
                .ToArray();
        }

        private static string NormalizeEnumText(string value) =>
            value.Replace(" ", string.Empty).Replace("-", string.Empty).ToLowerInvariant();

        public async Task<PagedResult<JobPosting>> GetPaginatedByCircularTypesAsync(
            PagedRequest request,
            IReadOnlyCollection<CircularTypeEnum> allowedCircularTypes,
            string? location,
            long? departmentId,
            EmploymentTypeEnum? employmentType,
            int? maxExperienceYears)
        {
            var query = ApplyAudienceFilter(_dbSet.Include(j => j.Department).AsQueryable(), allowedCircularTypes)
                .Where(j => location == null || (j.Location != null && j.Location.Contains(location)))
                .Where(j => departmentId == null || j.DepartmentId == departmentId)
                .Where(j => employmentType == null || j.EmploymentType == employmentType)
                .Where(j => maxExperienceYears == null || j.MinExperienceYears == null || j.MinExperienceYears <= maxExperienceYears);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var normalizedSearchTerm = request.SearchTerm.Trim().ToLowerInvariant();

                query = query.Where(j =>
                    j.Title.ToLower().Contains(normalizedSearchTerm) ||
                    j.JobPostingCode.ToLower().Contains(normalizedSearchTerm));
            }

            var orderedQuery = query
                .OrderByDescending(j => j.CreatedAt)
                .ThenByDescending(j => j.JobPostingId);

            return await orderedQuery.ToPaginatedResultAsync(request);
        }

        public async Task<JobPosting?> GetOpenByIdAndCircularTypesAsync(long jobPostingId, IReadOnlyCollection<CircularTypeEnum> allowedCircularTypes)
        {
            return await ApplyAudienceFilter(_dbSet.Include(j => j.Department).Include(j => j.Attachments).AsQueryable(), allowedCircularTypes)
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

        public async Task<List<JobPosting>> GetAllNewestFirstAsync()
        {
            return await _dbSet
                .Include(j => j.Applications)
                .Include(j => j.HiringPipeline)
                .Include(j => j.Department)
                .OrderByDescending(j => j.CreatedAt)
                .ThenByDescending(j => j.JobPostingId)
                .ToListAsync();
        }

        public async Task<List<JobPosting>> GetByCreatedByAsync(long userAccountId)
        {
            return await _dbSet
                .Include(j => j.Applications)
                .Include(j => j.HiringPipeline)
                .Include(j => j.Department)
                .Where(j => j.CreatedBy == userAccountId)
                .ToListAsync();
        }
    }
}
