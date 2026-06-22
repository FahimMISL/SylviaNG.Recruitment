using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IInterviewEvaluationScoreService
    {
        Task<long> CreateAsync(InterviewEvaluationScoreCreateRequest request);
        Task UpdateAsync(long interviewEvaluationScoreId, InterviewEvaluationScoreUpdateRequest request);
        Task DeleteAsync(long interviewEvaluationScoreId);
        Task<List<InterviewEvaluationScoreResponse>> GetAllAsync();
        Task<InterviewEvaluationScoreResponse> GetByIdAsync(long interviewEvaluationScoreId);
        Task<PagedResult<InterviewEvaluationScoreResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
