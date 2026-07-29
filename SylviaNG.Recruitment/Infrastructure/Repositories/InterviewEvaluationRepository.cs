using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class InterviewEvaluationRepository : Repository<InterviewEvaluation>, IInterviewEvaluationRepository
    {
        public InterviewEvaluationRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByInterviewAndEmployeeAsync(long interviewId, long employeeId, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(e =>
                e.InterviewId == interviewId &&
                e.EmployeeId == employeeId &&
                (!excludeId.HasValue || e.InterviewEvaluationId != excludeId.Value));
        }

        public async Task<List<InterviewEvaluation>> GetByInterviewIdAsync(long interviewId)
        {
            return await _dbSet
                .Include(e => e.Scorecard)
                    .ThenInclude(s => s.Criteria)
                .Include(e => e.Scores)
                .Where(e => e.InterviewId == interviewId)
                .OrderBy(e => e.SubmittedAt)
                .ToListAsync();
        }

        public async Task<InterviewEvaluation?> GetByIdWithDetailsAsync(long interviewEvaluationId)
        {
            return await _dbSet
                .Include(e => e.Scorecard)
                    .ThenInclude(s => s.Criteria)
                .Include(e => e.Scores)
                .FirstOrDefaultAsync(e => e.InterviewEvaluationId == interviewEvaluationId);
        }

        public async Task<List<InterviewEvaluation>> GetForAnalyticsScopeAsync(
            long? jobPostingId, long? departmentId, DateTime? dateFrom, DateTime? dateTo)
        {
            return await _dbSet
                .Include(e => e.Scorecard)
                    .ThenInclude(s => s.Criteria)
                .Include(e => e.Scores)
                .Include(e => e.Interview)
                    .ThenInclude(i => i.JobApplication)
                        .ThenInclude(a => a.JobPosting)
                .Where(e => jobPostingId == null || e.Interview.JobApplication.JobPostingId == jobPostingId)
                .Where(e => departmentId == null || e.Interview.JobApplication.JobPosting.DepartmentId == departmentId)
                .Where(e => dateFrom == null || e.SubmittedAt >= dateFrom)
                .Where(e => dateTo == null || e.SubmittedAt <= dateTo)
                .ToListAsync();
        }
    }
}
