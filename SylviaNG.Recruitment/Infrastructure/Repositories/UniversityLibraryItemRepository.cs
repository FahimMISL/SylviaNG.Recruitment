using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class UniversityLibraryItemRepository : Repository<UniversityLibraryItem>, IUniversityLibraryItemRepository
    {
        public UniversityLibraryItemRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<UniversityLibraryItem>> GetAllOrderedAsync()
        {
            return await _dbSet.OrderBy(u => u.Name).ToListAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(u => u.Name == name && (!excludeId.HasValue || u.UniversityLibraryItemId != excludeId.Value));
        }

        public async Task<int> CountUsageAsync(long universityLibraryItemId)
        {
            return await _dbContext.CandidateEducations.CountAsync(e => e.UniversityLibraryItemId == universityLibraryItemId);
        }
    }
}
