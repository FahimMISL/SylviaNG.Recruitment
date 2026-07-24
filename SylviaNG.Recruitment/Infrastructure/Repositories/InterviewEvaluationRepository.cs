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
    }
}
