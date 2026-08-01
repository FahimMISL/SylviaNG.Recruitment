using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class QuestionGroupRepository : Repository<QuestionGroup>, IQuestionGroupRepository
    {
        public QuestionGroupRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(g => g.Name == name && (!excludeId.HasValue || g.QuestionGroupId != excludeId.Value));
        }

        public async Task<List<QuestionGroup>> GetActiveAsync()
        {
            return await _dbSet
                .Where(g => g.IsActive)
                .OrderBy(g => g.Name)
                .ToListAsync();
        }
    }
}
