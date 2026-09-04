using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class AutoShortlistRunRepository : Repository<AutoShortlistRun>, IAutoShortlistRunRepository
    {
        public AutoShortlistRunRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<AutoShortlistRun?> GetLatestByJobPostingIdAsync(long jobPostingId)
        {
            return await _dbSet
                .Include(r => r.Results)
                .Where(r => r.JobPostingId == jobPostingId)
                .OrderByDescending(r => r.RunAt)
                .FirstOrDefaultAsync();
        }

        public async Task<AutoShortlistRun?> GetByIdWithResultsAsync(long autoShortlistRunId)
        {
            return await _dbSet
                .Include(r => r.Results)
                .FirstOrDefaultAsync(r => r.AutoShortlistRunId == autoShortlistRunId);
        }

        public async Task<AutoShortlistResult?> GetResultByIdAsync(long autoShortlistResultId)
        {
            return await _dbContext.Set<AutoShortlistResult>()
                .Include(r => r.AutoShortlistRun)
                .FirstOrDefaultAsync(r => r.AutoShortlistResultId == autoShortlistResultId);
        }

        public void UpdateResult(AutoShortlistResult result)
        {
            _dbContext.Set<AutoShortlistResult>().Update(result);
        }

        public async Task<Dictionary<long, int>> GetLatestScoresByJobPostingIdAsync(long jobPostingId)
        {
            var latestRun = await GetLatestByJobPostingIdAsync(jobPostingId);
            if (latestRun == null)
                return new Dictionary<long, int>();

            return latestRun.Results
                .Where(r => r.Score.HasValue)
                .ToDictionary(r => r.JobApplicationId, r => r.Score!.Value);
        }
    }
}
