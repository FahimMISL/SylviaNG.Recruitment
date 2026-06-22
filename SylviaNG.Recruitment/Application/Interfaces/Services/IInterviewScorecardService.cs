using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.InterviewScorecards.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IInterviewScorecardService
    {
        Task<long> CreateAsync(InterviewScorecardCreateRequest request);
        Task UpdateAsync(long interviewScorecardId, InterviewScorecardUpdateRequest request);
        Task DeleteAsync(long interviewScorecardId);
        Task<List<InterviewScorecardResponse>> GetAllAsync();
        Task<InterviewScorecardResponse> GetByIdAsync(long interviewScorecardId);
        Task<PagedResult<InterviewScorecardResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
