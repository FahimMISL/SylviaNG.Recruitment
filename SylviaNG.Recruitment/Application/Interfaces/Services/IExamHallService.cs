using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IExamHallService
    {
        Task<long> CreateAsync(ExamHallCreateRequest request);
        Task UpdateAsync(long examHallId, ExamHallUpdateRequest request);
        Task DeleteAsync(long examHallId);
        Task<List<ExamHallResponse>> GetAllAsync();
        Task<ExamHallResponse> GetByIdAsync(long examHallId);
        Task<PagedResult<ExamHallResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
