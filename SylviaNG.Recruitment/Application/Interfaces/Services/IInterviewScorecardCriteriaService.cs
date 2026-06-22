using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IInterviewScorecardCriteriaService
    {
        Task<long> CreateAsync(InterviewScorecardCriteriaCreateRequest request);
        Task UpdateAsync(long interviewScorecardCriteriaId, InterviewScorecardCriteriaUpdateRequest request);
        Task DeleteAsync(long interviewScorecardCriteriaId);
        Task<List<InterviewScorecardCriteriaResponse>> GetAllAsync();
        Task<InterviewScorecardCriteriaResponse> GetByIdAsync(long interviewScorecardCriteriaId);
        Task<PagedResult<InterviewScorecardCriteriaResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
