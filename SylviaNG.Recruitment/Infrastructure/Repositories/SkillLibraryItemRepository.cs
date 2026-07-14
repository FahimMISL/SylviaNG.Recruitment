using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class SkillLibraryItemRepository : Repository<SkillLibraryItem>, ISkillLibraryItemRepository
    {
        public SkillLibraryItemRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<SkillLibraryItem>> GetAllOrderedAsync()
        {
            return await _dbSet.OrderBy(s => s.Name).ToListAsync();
        }
    }
}
