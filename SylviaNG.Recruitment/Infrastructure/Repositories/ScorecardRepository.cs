using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class ScorecardRepository : Repository<Scorecard>, IScorecardRepository
    {
        public ScorecardRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(s => s.Name == name && (!excludeId.HasValue || s.ScorecardId != excludeId.Value));
        }

        public async Task<Scorecard?> GetByIdWithCriteriaAsync(long scorecardId)
        {
            return await _dbSet
                .Include(s => s.Criteria.OrderBy(c => c.DisplayOrder))
                .FirstOrDefaultAsync(s => s.ScorecardId == scorecardId);
        }

        public async Task<List<Scorecard>> GetAllWithCriteriaAsync()
        {
            return await _dbSet
                .Include(s => s.Criteria.OrderBy(c => c.DisplayOrder))
                .ToListAsync();
        }

        public async Task<List<Scorecard>> GetActiveAsync()
        {
            return await _dbSet
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
    }
}
