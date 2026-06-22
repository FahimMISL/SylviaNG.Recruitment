using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.InterviewSessions.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IInterviewSessionService
    {
        Task<long> CreateAsync(InterviewSessionCreateRequest request);
        Task UpdateAsync(long interviewSessionId, InterviewSessionUpdateRequest request);
        Task DeleteAsync(long interviewSessionId);
        Task<List<InterviewSessionResponse>> GetAllAsync();
        Task<InterviewSessionResponse> GetByIdAsync(long interviewSessionId);
        Task<PagedResult<InterviewSessionResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
