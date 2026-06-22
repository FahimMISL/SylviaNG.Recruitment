using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.Exams.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IExamService
    {
        Task<long> CreateAsync(ExamCreateRequest request);
        Task UpdateAsync(long examId, ExamUpdateRequest request);
        Task DeleteAsync(long examId);
        Task<List<ExamResponse>> GetAllAsync();
        Task<ExamResponse> GetByIdAsync(long examId);
        Task<PagedResult<ExamResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
