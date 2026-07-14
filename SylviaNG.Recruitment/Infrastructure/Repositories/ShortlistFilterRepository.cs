using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class ShortlistFilterRepository : Repository<ShortlistFilter>, IShortlistFilterRepository
    {
        public ShortlistFilterRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(f => f.Name == name && (!excludeId.HasValue || f.ShortlistFilterId != excludeId.Value));
        }

        public async Task<ShortlistFilter?> GetByIdWithCriteriaAsync(long shortlistFilterId)
        {
            return await _dbSet
                .Include(f => f.Criteria.OrderBy(c => c.DisplayOrder))
                .FirstOrDefaultAsync(f => f.ShortlistFilterId == shortlistFilterId);
        }

        public async Task<List<ShortlistFilter>> GetAllWithCriteriaAsync()
        {
            return await _dbSet
                .Include(f => f.Criteria.OrderBy(c => c.DisplayOrder))
                .ToListAsync();
        }

        public async Task<List<ShortlistFilter>> GetActiveAsync()
        {
            return await _dbSet
                .Where(f => f.IsActive)
                .OrderBy(f => f.Name)
                .ToListAsync();
        }
    }
}
