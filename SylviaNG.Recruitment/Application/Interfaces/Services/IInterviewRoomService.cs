using SylviaNG.Recruitment.Application.Features.InterviewRooms.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IInterviewRoomService
    {
        Task<long> CreateAsync(long interviewVenueId, InterviewRoomCreateRequest request);
        Task UpdateAsync(long interviewRoomId, InterviewRoomUpdateRequest request);
        Task SetActiveStatusAsync(long interviewRoomId, bool isActive);
        Task<InterviewRoomResponse> GetByIdAsync(long interviewRoomId);
        Task<List<InterviewRoomResponse>> GetAllByVenueIdAsync(long interviewVenueId);
    }
}
