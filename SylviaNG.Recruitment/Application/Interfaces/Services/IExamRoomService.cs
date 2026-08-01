using SylviaNG.Recruitment.Application.Features.ExamRooms.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IExamRoomService
    {
        Task<long> CreateAsync(long examVenueId, ExamRoomCreateRequest request);
        Task UpdateAsync(long examRoomId, ExamRoomUpdateRequest request);
        Task SetActiveStatusAsync(long examRoomId, bool isActive);
        Task<ExamRoomResponse> GetByIdAsync(long examRoomId);
        Task<List<ExamRoomResponse>> GetAllByVenueIdAsync(long examVenueId);
    }
}
