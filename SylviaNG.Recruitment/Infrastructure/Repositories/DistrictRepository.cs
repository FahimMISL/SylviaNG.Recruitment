using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class DistrictRepository : Repository<District>, IDistrictRepository
    {
        public DistrictRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<District>> GetByDivisionIdOrderedAsync(long divisionId)
        {
            return await _dbSet.Where(d => d.DivisionId == divisionId).OrderBy(d => d.Name).ToListAsync();
        }
    }
}
