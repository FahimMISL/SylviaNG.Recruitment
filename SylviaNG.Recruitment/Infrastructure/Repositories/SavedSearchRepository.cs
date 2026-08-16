using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class SavedSearchRepository : Repository<SavedSearch>, ISavedSearchRepository
    {
        public SavedSearchRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameForOwnerAsync(string ownerUserName, string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(s =>
                s.OwnerUserName == ownerUserName &&
                s.Name == name &&
                (!excludeId.HasValue || s.SavedSearchId != excludeId.Value));
        }

        public async Task<List<SavedSearch>> GetVisibleAsync(string ownerUserName)
        {
            return await _dbSet
                .Where(s => s.OwnerUserName == ownerUserName || s.IsShared)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
    }
}
