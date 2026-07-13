using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
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
    }
}
