using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IExamQuestionRepository : IRepository<ExamQuestion>
    {
        /// <summary>Full aggregate load: question and its options (ordered), for edit/detail (US-053).</summary>
        Task<ExamQuestion?> GetByIdWithOptionsAsync(long examQuestionId);

        /// <summary>Searched/filtered/paged list (US-053 AC5).</summary>
        Task<PagedResult<ExamQuestion>> GetPaginatedAsync(
            PagedRequest request,
            long? questionGroupId,
            QuestionTypeEnum? questionType,
            DifficultyLevelEnum? difficultyLevel,
            bool? isActive);
    }
}
