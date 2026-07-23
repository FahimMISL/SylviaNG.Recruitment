using SylviaNG.Recruitment.Application.Features.ExamVenues.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    /// <summary>
    /// Manual mapping methods for ExamVenue. No AutoMapper, matching ExamHallMapper.
    /// </summary>
    public static class ExamVenueMapper
    {
        public static ExamVenue ToEntity(this ExamVenueCreateRequest request)
        {
            return new ExamVenue
            {
                VenueName = request.VenueName,
                Location = request.Location,
                IsActive = true,
            };
        }

        public static ExamVenueResponse ToResponse(this ExamVenue entity, int roomCount)
        {
            return new ExamVenueResponse
            {
                ExamVenueId = entity.ExamVenueId,
                VenueName = entity.VenueName,
                Location = entity.Location,
                IsActive = entity.IsActive,
                RoomCount = roomCount,
            };
        }

        public static ExamVenueLookupResponse ToLookupResponse(this ExamVenue entity)
        {
            return new ExamVenueLookupResponse
            {
                ExamVenueId = entity.ExamVenueId,
                VenueName = entity.VenueName,
            };
        }
    }
}
