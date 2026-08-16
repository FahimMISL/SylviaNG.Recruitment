using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class EducationBoardRepository : Repository<EducationBoard>, IEducationBoardRepository
    {
        public EducationBoardRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(b => b.Name == name && (!excludeId.HasValue || b.EducationBoardId != excludeId.Value));
        }

        public async Task<List<EducationBoard>> GetAllOrderedAsync()
        {
            return await _dbSet.OrderBy(b => b.Name).ToListAsync();
        }

        public async Task<int> CountUsageAsync(long educationBoardId)
        {
            return await _dbContext.CandidateEducations.CountAsync(e => e.EducationBoardId == educationBoardId);
        }
    }
}
