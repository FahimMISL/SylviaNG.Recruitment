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

        /// <summary>Every active question (with options, ordered by ExamQuestionId) across one or
        /// more groups - assembles a candidate's exam paper (US-058), now that an exam can draw
        /// from multiple question groups. No DisplayOrder exists on ExamQuestion itself, unlike
        /// its Options.</summary>
        Task<List<ExamQuestion>> GetActiveByQuestionGroupIdsAsync(IReadOnlyList<long> questionGroupIds);

        /// <summary>Active-question count per group, for surfacing next to each group in the
        /// exam-schedule picker - lets HR see a group is empty before picking it (an exam whose
        /// only linked group has 0 active questions ships with an empty paper, see
        /// ExamTakingService.StartExamAsync).</summary>
        Task<Dictionary<long, int>> CountActiveByQuestionGroupIdsAsync(IReadOnlyList<long> questionGroupIds);

        /// <summary>Searched/filtered/paged list (US-053 AC5).</summary>
        Task<PagedResult<ExamQuestion>> GetPaginatedAsync(
            PagedRequest request,
            long? questionGroupId,
            QuestionTypeEnum? questionType,
            DifficultyLevelEnum? difficultyLevel,
            bool? isActive);
    }
}
