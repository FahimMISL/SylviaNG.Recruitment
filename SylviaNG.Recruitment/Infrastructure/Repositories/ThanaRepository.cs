using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class ThanaRepository : Repository<Thana>, IThanaRepository
    {
        public ThanaRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<Thana>> GetByDistrictIdOrderedAsync(long districtId)
        {
            return await _dbSet.Where(t => t.DistrictId == districtId).OrderBy(t => t.Name).ToListAsync();
        }
    }
}
