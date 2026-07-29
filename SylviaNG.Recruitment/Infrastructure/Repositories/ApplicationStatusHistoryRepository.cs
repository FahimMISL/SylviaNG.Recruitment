using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class ApplicationStatusHistoryRepository : Repository<ApplicationStatusHistory>, IApplicationStatusHistoryRepository
    {
        public ApplicationStatusHistoryRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<ApplicationStatusHistory>> GetForApplicationsAsync(IEnumerable<long> jobApplicationIds)
        {
            var ids = jobApplicationIds.ToList();

            return await _dbSet
                .Include(h => h.Reason)
                .Where(h => ids.Contains(h.JobApplicationId))
                .OrderBy(h => h.JobApplicationId)
                .ThenBy(h => h.ChangedAt)
                .ToListAsync();
        }
    }
}
