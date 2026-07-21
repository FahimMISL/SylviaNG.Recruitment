using SylviaNG.Recruitment.Application.Features.ExamQuestions.Models;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IExamQuestionService
    {
        Task<long> CreateAsync(ExamQuestionCreateRequest request);
        Task UpdateAsync(long examQuestionId, ExamQuestionUpdateRequest request);
        Task SetActiveStatusAsync(long examQuestionId, bool isActive);
        Task<ExamQuestionResponse> GetByIdAsync(long examQuestionId);
        Task<PagedResult<ExamQuestionResponse>> GetPaginatedAsync(
            PagedRequest request,
            long? questionGroupId,
            QuestionTypeEnum? questionType,
            DifficultyLevelEnum? difficultyLevel,
            bool? isActive);
    }
}
