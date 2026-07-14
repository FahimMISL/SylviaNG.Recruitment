using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class JobApplicationRepository : Repository<JobApplication>, IJobApplicationRepository
    {
        public JobApplicationRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<JobApplication?> GetByEmailAndJobPostingIdAsync(string email, long jobPostingId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(a => a.CandidateEmail == email && a.JobPostingId == jobPostingId);
        }

        public async Task<PagedResult<JobApplication>> GetPaginatedByJobPostingAsync(long jobPostingId, PagedRequest request)
        {
            var query = _dbSet
                .Include(a => a.Interviews)
                .Where(a => a.JobPostingId == jobPostingId)
                .AsQueryable();

            return await query.ToPaginatedResultAsync(request);
        }

        public async Task<int> CountAllAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<List<JobApplication>> GetByCandidateEmailAsync(string email)
        {
            return await _dbSet
                .Include(a => a.JobPosting)
                .Include(a => a.Interviews)
                .Where(a => a.CandidateEmail == email)
                .OrderByDescending(a => a.AppliedDate)
                .ToListAsync();
        }

        public async Task<PagedResult<JobApplication>> GetPaginatedAllAsync(
            PagedRequest request,
            long? jobPostingId,
            ApplicationStatusEnum? status,
            ApplicationSourceEnum? source,
            DateTime? dateFrom,
            DateTime? dateTo)
        {
            var query = BuildDashboardFilterQuery(jobPostingId, status, source, dateFrom, dateTo)
                .Include(a => a.JobPosting);

            return await query.ToPaginatedResultAsync(request);
        }

        public async Task<List<long>> GetAllMatchingIdsAsync(
            long? jobPostingId,
            ApplicationStatusEnum? status,
            ApplicationSourceEnum? source,
            DateTime? dateFrom,
            DateTime? dateTo)
        {
            return await BuildDashboardFilterQuery(jobPostingId, status, source, dateFrom, dateTo)
                .Select(a => a.JobApplicationId)
                .ToListAsync();
        }

        private IQueryable<JobApplication> BuildDashboardFilterQuery(
            long? jobPostingId,
            ApplicationStatusEnum? status,
            ApplicationSourceEnum? source,
            DateTime? dateFrom,
            DateTime? dateTo)
        {
            return _dbSet
                .Where(a => jobPostingId == null || a.JobPostingId == jobPostingId)
                .Where(a => status == null || a.ApplicationStatus == status)
                .Where(a => source == null || a.Source == source)
                .Where(a => dateFrom == null || a.AppliedDate == null || a.AppliedDate >= dateFrom)
                .Where(a => dateTo == null || a.AppliedDate == null || a.AppliedDate <= dateTo);
        }

        public async Task<JobApplication?> GetByIdWithHistoryAsync(long jobApplicationId)
        {
            return await _dbSet
                .Include(a => a.JobPosting)
                .Include(a => a.StatusHistory.OrderByDescending(h => h.ChangedAt))
                    .ThenInclude(h => h.Reason)
                .FirstOrDefaultAsync(a => a.JobApplicationId == jobApplicationId);
        }

        public async Task<List<JobApplication>> GetAllByJobPostingIdAsync(long jobPostingId)
        {
            return await _dbSet
                .Where(a => a.JobPostingId == jobPostingId)
                .ToListAsync();
        }

        public async Task<List<JobApplication>> GetAllByJobPostingAndScalarFiltersAsync(
            long jobPostingId,
            ApplicationStatusEnum? status,
            ApplicationSourceEnum? source,
            DateTime? dateFrom,
            DateTime? dateTo)
        {
            return await BuildDashboardFilterQuery(jobPostingId, status, source, dateFrom, dateTo)
                .ToListAsync();
        }
    }
}
