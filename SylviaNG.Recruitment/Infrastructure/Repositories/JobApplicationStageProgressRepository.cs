using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class JobApplicationStageProgressRepository : Repository<JobApplicationStageProgress>, IJobApplicationStageProgressRepository
    {
        public JobApplicationStageProgressRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<JobApplicationStageProgress>> GetByJobApplicationIdAsync(long jobApplicationId)
        {
            return await _dbSet
                .Where(p => p.JobApplicationId == jobApplicationId)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();
        }

        public async Task<Dictionary<long, JobApplicationStageProgress>> GetCurrentByJobApplicationIdsAsync(List<long> jobApplicationIds)
        {
            if (jobApplicationIds.Count == 0)
                return new Dictionary<long, JobApplicationStageProgress>();

            var rows = await _dbSet
                .Where(p => jobApplicationIds.Contains(p.JobApplicationId)
                    && (p.Status == StageProgressStatusEnum.InProgress || p.Status == StageProgressStatusEnum.Completed))
                .ToListAsync();

            return rows
                .GroupBy(p => p.JobApplicationId)
                .ToDictionary(
                    g => g.Key,
                    g => g.FirstOrDefault(p => p.Status == StageProgressStatusEnum.InProgress)
                        ?? g.OrderByDescending(p => p.DisplayOrder).First());
        }

        public async Task<int> CountPendingApprovalsAsync()
        {
            return await _dbSet.CountAsync(p => p.Status == StageProgressStatusEnum.InProgress && p.RequiresManualApproval);
        }
    }
}
