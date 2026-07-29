using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class GenderRepository : Repository<Gender>, IGenderRepository
    {
        public GenderRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(g => g.Name == name && (!excludeId.HasValue || g.GenderId != excludeId.Value));
        }

        public async Task<List<Gender>> GetAllOrderedAsync()
        {
            return await _dbSet.OrderBy(g => g.Name).ToListAsync();
        }

        public async Task<int> CountUsageAsync(long genderId)
        {
            return await _dbContext.CandidateProfiles.CountAsync(p => p.GenderId == genderId);
        }
    }
}
