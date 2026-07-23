using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class DegreeRepository : Repository<Degree>, IDegreeRepository
    {
        public DegreeRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(d => d.Name == name && (!excludeId.HasValue || d.DegreeId != excludeId.Value));
        }

        public async Task<List<Degree>> GetAllOrderedAsync()
        {
            return await _dbSet.OrderBy(d => d.Position).ThenBy(d => d.Name).ToListAsync();
        }

        public async Task<int> CountUsageAsync(long degreeId)
        {
            return await _dbContext.CandidateEducations.CountAsync(e => e.DegreeId == degreeId);
        }
    }
}
