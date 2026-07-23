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

        /// <summary>Every active question (with options, ordered by ExamQuestionId) in a group -
        /// assembles a candidate's exam paper (US-058). No DisplayOrder exists on ExamQuestion
        /// itself, unlike its Options.</summary>
        Task<List<ExamQuestion>> GetActiveByQuestionGroupIdAsync(long questionGroupId);

        /// <summary>Searched/filtered/paged list (US-053 AC5).</summary>
        Task<PagedResult<ExamQuestion>> GetPaginatedAsync(
            PagedRequest request,
            long? questionGroupId,
            QuestionTypeEnum? questionType,
            DifficultyLevelEnum? difficultyLevel,
            bool? isActive);
    }
}
