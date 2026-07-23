using SylviaNG.Recruitment.Application.Features.InterviewRooms.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    /// <summary>Manual mapping methods for InterviewRoom. No AutoMapper, matching ExamRoomMapper.</summary>
    public static class InterviewRoomMapper
    {
        public static InterviewRoom ToEntity(this InterviewRoomCreateRequest request, long interviewVenueId)
        {
            return new InterviewRoom
            {
                InterviewVenueId = interviewVenueId,
                RoomName = request.RoomName,
                Capacity = request.Capacity,
                IsActive = true,
            };
        }

        public static InterviewRoomResponse ToResponse(this InterviewRoom entity)
        {
            return new InterviewRoomResponse
            {
                InterviewRoomId = entity.InterviewRoomId,
                InterviewVenueId = entity.InterviewVenueId,
                RoomName = entity.RoomName,
                Capacity = entity.Capacity,
                IsActive = entity.IsActive,
            };
        }
    }
}
