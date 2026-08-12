using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class ExamRepository : Repository<Exam>, IExamRepository
    {
        public ExamRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<PagedResult<Exam>> GetPagedAsync(PagedRequest request, long? jobPostingId, ExamTypeEnum? examType, bool? isActive)
        {
            var query = _dbSet
                .Include(e => e.ExamVenue)
                .Include(e => e.QuestionGroupLinks)
                    .ThenInclude(l => l.QuestionGroup)
                .AsQueryable();

            if (jobPostingId.HasValue)
                query = query.Where(e => e.JobPostingId == jobPostingId.Value);

            if (examType.HasValue)
                query = query.Where(e => e.ExamType == examType.Value);

            if (isActive.HasValue)
                query = query.Where(e => e.IsActive == isActive.Value);

            request.SearchProperties = [nameof(Exam.Title)];

            var orderedQuery = query.OrderByDescending(e => e.ScheduledStartAt);

            return await orderedQuery.ToPaginatedResultAsync(request);
        }

        public async Task<Exam?> GetByIdWithDetailsAsync(long examId)
        {
            return await _dbSet
                .Include(e => e.ExamVenue)
                .Include(e => e.QuestionGroupLinks)
                    .ThenInclude(l => l.QuestionGroup)
                .FirstOrDefaultAsync(e => e.ExamId == examId);
        }
    }
}
