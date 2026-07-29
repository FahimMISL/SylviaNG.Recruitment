using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class MajorSubjectUniversityRepository : Repository<MajorSubjectUniversity>, IMajorSubjectUniversityRepository
    {
        public MajorSubjectUniversityRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(m => m.Name == name && (!excludeId.HasValue || m.MajorSubjectUniversityId != excludeId.Value));
        }

        public async Task<List<MajorSubjectUniversity>> GetAllOrderedAsync()
        {
            return await _dbSet.OrderBy(m => m.Name).ToListAsync();
        }

        public async Task<int> CountUsageAsync(long majorSubjectUniversityId)
        {
            return await _dbContext.CandidateEducations.CountAsync(e => e.MajorSubjectUniversityId == majorSubjectUniversityId);
        }
    }
}
