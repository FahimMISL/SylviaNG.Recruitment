using SylviaNG.Recruitment.Application.Features.InterviewVenues.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IInterviewVenueService
    {
        Task<long> CreateAsync(InterviewVenueCreateRequest request);
        Task UpdateAsync(long interviewVenueId, InterviewVenueUpdateRequest request);
        Task SetActiveStatusAsync(long interviewVenueId, bool isActive);
        Task<InterviewVenueResponse> GetByIdAsync(long interviewVenueId);
        Task<List<InterviewVenueResponse>> GetAllAsync();
        Task<List<InterviewVenueLookupResponse>> GetActiveLookupAsync();
    }
}
