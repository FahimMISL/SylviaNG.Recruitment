using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class CountryRepository : Repository<Country>, ICountryRepository
    {
        public CountryRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(c => c.Name == name && (!excludeId.HasValue || c.CountryId != excludeId.Value));
        }

        public async Task<bool> ExistsByCodeAsync(string code, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(c => c.Code == code && (!excludeId.HasValue || c.CountryId != excludeId.Value));
        }

        public async Task<List<Country>> GetAllOrderedAsync()
        {
            return await _dbSet.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<int> CountUsageAsync(long countryId)
        {
            return await _dbContext.CandidateProfiles.CountAsync(p => p.CountryId == countryId);
        }
    }
}
