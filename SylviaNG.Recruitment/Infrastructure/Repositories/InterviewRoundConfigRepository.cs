using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class InterviewRoundConfigRepository : Repository<InterviewRoundConfig>, IInterviewRoundConfigRepository
    {
        public InterviewRoundConfigRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<InterviewRoundConfig>> GetAllByJobPostingIdAsync(long jobPostingId)
        {
            return await _dbSet
                .Include(r => r.Scorecard)
                .Include(r => r.PanelMembers)
                .Where(r => r.JobPostingId == jobPostingId)
                .OrderBy(r => r.Sequence)
                .ToListAsync();
        }

        public async Task<InterviewRoundConfig?> GetByIdWithDetailsAsync(long interviewRoundConfigId)
        {
            return await _dbSet
                .Include(r => r.Scorecard)
                .Include(r => r.PanelMembers)
                .FirstOrDefaultAsync(r => r.InterviewRoundConfigId == interviewRoundConfigId);
        }

        public async Task<InterviewRoundConfig?> GetByJobPostingAndSequenceAsync(long jobPostingId, int sequence)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.JobPostingId == jobPostingId && r.Sequence == sequence);
        }
    }
}
