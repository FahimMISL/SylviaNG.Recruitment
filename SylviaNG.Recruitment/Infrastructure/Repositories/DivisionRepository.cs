using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class DivisionRepository : Repository<Division>, IDivisionRepository
    {
        public DivisionRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<Division>> GetAllOrderedAsync()
        {
            return await _dbSet.OrderBy(d => d.Name).ToListAsync();
        }
    }
}
