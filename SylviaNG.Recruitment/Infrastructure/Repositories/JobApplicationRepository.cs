using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
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

        public async Task<PagedResult<JobApplication>> GetAllPaginatedAsync(PagedRequest request)
        {
            var query = _dbSet
                .Include(a => a.JobPosting)
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.AppliedDate)
                .AsQueryable();

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
