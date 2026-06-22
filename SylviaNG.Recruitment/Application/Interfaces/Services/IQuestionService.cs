using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.Questions.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IQuestionService
    {
        Task<long> CreateAsync(QuestionCreateRequest request);
        Task UpdateAsync(long questionId, QuestionUpdateRequest request);
        Task DeleteAsync(long questionId);
        Task<List<QuestionResponse>> GetAllAsync();
        Task<QuestionResponse> GetByIdAsync(long questionId);
        Task<PagedResult<QuestionResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
