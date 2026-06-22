using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IExamSeatPlanService
    {
        Task<long> CreateAsync(ExamSeatPlanCreateRequest request);
        Task UpdateAsync(long examSeatPlanId, ExamSeatPlanUpdateRequest request);
        Task DeleteAsync(long examSeatPlanId);
        Task<List<ExamSeatPlanResponse>> GetAllAsync();
        Task<ExamSeatPlanResponse> GetByIdAsync(long examSeatPlanId);
        Task<PagedResult<ExamSeatPlanResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
