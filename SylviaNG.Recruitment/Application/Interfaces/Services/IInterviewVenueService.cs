using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IInterviewVenueService
    {
        Task<long> CreateAsync(InterviewVenueCreateRequest request);
        Task UpdateAsync(long interviewVenueId, InterviewVenueUpdateRequest request);
        Task DeleteAsync(long interviewVenueId);
        Task<List<InterviewVenueResponse>> GetAllAsync();
        Task<InterviewVenueResponse> GetByIdAsync(long interviewVenueId);
        Task<PagedResult<InterviewVenueResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
