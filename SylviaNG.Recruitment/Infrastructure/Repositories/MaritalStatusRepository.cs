using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class MaritalStatusRepository : Repository<MaritalStatus>, IMaritalStatusRepository
    {
        public MaritalStatusRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(g => g.Name == name && (!excludeId.HasValue || g.MaritalStatusId != excludeId.Value));
        }

        public async Task<List<MaritalStatus>> GetAllOrderedAsync()
        {
            return await _dbSet.OrderBy(g => g.Name).ToListAsync();
        }

        public async Task<int> CountUsageAsync(long maritalStatusId)
        {
            return await _dbContext.CandidateProfiles.CountAsync(p => p.MaritalStatusId == maritalStatusId);
        }
    }
}
