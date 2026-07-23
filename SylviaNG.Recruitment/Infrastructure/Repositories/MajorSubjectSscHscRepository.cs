using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class MajorSubjectSscHscRepository : Repository<MajorSubjectSscHsc>, IMajorSubjectSscHscRepository
    {
        public MajorSubjectSscHscRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(m => m.Name == name && (!excludeId.HasValue || m.MajorSubjectSscHscId != excludeId.Value));
        }

        public async Task<List<MajorSubjectSscHsc>> GetAllOrderedAsync()
        {
            return await _dbSet.OrderBy(m => m.Name).ToListAsync();
        }

        public async Task<int> CountUsageAsync(long majorSubjectSscHscId)
        {
            return await _dbContext.CandidateEducations.CountAsync(e => e.MajorSubjectSscHscId == majorSubjectSscHscId);
        }
    }
}
