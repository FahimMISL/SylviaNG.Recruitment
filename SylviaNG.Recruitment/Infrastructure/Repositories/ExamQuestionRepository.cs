using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class ExamQuestionRepository : Repository<ExamQuestion>, IExamQuestionRepository
    {
        public ExamQuestionRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<ExamQuestion?> GetByIdWithOptionsAsync(long examQuestionId)
        {
            return await _dbSet
                .Include(q => q.Options.OrderBy(o => o.DisplayOrder))
                .FirstOrDefaultAsync(q => q.ExamQuestionId == examQuestionId);
        }

        public async Task<List<ExamQuestion>> GetActiveByQuestionGroupIdAsync(long questionGroupId)
        {
            return await _dbSet
                .Include(q => q.Options.OrderBy(o => o.DisplayOrder))
                .Where(q => q.QuestionGroupId == questionGroupId && q.IsActive)
                .OrderBy(q => q.ExamQuestionId)
                .ToListAsync();
        }

        public async Task<PagedResult<ExamQuestion>> GetPaginatedAsync(
            PagedRequest request,
            long? questionGroupId,
            QuestionTypeEnum? questionType,
            DifficultyLevelEnum? difficultyLevel,
            bool? isActive)
        {
            var query = _dbSet
                .Include(q => q.Options.OrderBy(o => o.DisplayOrder))
                .Include(q => q.QuestionGroup)
                .Where(q => questionGroupId == null || q.QuestionGroupId == questionGroupId)
                .Where(q => questionType == null || q.QuestionType == questionType)
                .Where(q => difficultyLevel == null || q.DifficultyLevel == difficultyLevel)
                .Where(q => isActive == null || q.IsActive == isActive)
                .AsQueryable();

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
