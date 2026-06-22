using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IInterviewEvaluationService
    {
        Task<long> CreateAsync(InterviewEvaluationCreateRequest request);
        Task UpdateAsync(long interviewEvaluationId, InterviewEvaluationUpdateRequest request);
        Task DeleteAsync(long interviewEvaluationId);
        Task<List<InterviewEvaluationResponse>> GetAllAsync();
        Task<InterviewEvaluationResponse> GetByIdAsync(long interviewEvaluationId);
        Task<PagedResult<InterviewEvaluationResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
