using SylviaNG.Recruitment.Application.Features.ExamRooms.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    /// <summary>
    /// Manual mapping methods for ExamRoom. No AutoMapper, matching ExamHallMapper.
    /// </summary>
    public static class ExamRoomMapper
    {
        public static ExamRoom ToEntity(this ExamRoomCreateRequest request, long examVenueId)
        {
            return new ExamRoom
            {
                ExamVenueId = examVenueId,
                RoomName = request.RoomName,
                Capacity = request.Capacity,
                RequiredInvigilatorCount = request.RequiredInvigilatorCount,
                IsActive = true,
            };
        }

        public static ExamRoomResponse ToResponse(this ExamRoom entity)
        {
            return new ExamRoomResponse
            {
                ExamRoomId = entity.ExamRoomId,
                ExamVenueId = entity.ExamVenueId,
                RoomName = entity.RoomName,
                Capacity = entity.Capacity,
                RequiredInvigilatorCount = entity.RequiredInvigilatorCount,
                IsActive = entity.IsActive,
            };
        }
    }
}
