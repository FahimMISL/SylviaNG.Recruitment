using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.QuestionOptions.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IQuestionOptionService
    {
        Task<long> CreateAsync(QuestionOptionCreateRequest request);
        Task UpdateAsync(long questionOptionId, QuestionOptionUpdateRequest request);
        Task DeleteAsync(long questionOptionId);
        Task<List<QuestionOptionResponse>> GetAllAsync();
        Task<QuestionOptionResponse> GetByIdAsync(long questionOptionId);
        Task<PagedResult<QuestionOptionResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
