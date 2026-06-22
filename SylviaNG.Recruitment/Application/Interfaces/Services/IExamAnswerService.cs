using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ExamAnswers.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IExamAnswerService
    {
        Task<long> CreateAsync(ExamAnswerCreateRequest request);
        Task UpdateAsync(long examAnswerId, ExamAnswerUpdateRequest request);
        Task DeleteAsync(long examAnswerId);
        Task<List<ExamAnswerResponse>> GetAllAsync();
        Task<ExamAnswerResponse> GetByIdAsync(long examAnswerId);
        Task<PagedResult<ExamAnswerResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
