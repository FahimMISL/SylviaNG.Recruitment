using SylviaNG.Recruitment.Application.Features.InterviewVenues.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    /// <summary>Manual mapping methods for InterviewVenue. No AutoMapper, matching ExamVenueMapper.</summary>
    public static class InterviewVenueMapper
    {
        public static InterviewVenue ToEntity(this InterviewVenueCreateRequest request)
        {
            return new InterviewVenue
            {
                VenueName = request.VenueName,
                Location = request.Location,
                IsActive = true,
            };
        }

        public static InterviewVenueResponse ToResponse(this InterviewVenue entity, int roomCount)
        {
            return new InterviewVenueResponse
            {
                InterviewVenueId = entity.InterviewVenueId,
                VenueName = entity.VenueName,
                Location = entity.Location,
                IsActive = entity.IsActive,
                RoomCount = roomCount,
            };
        }

        public static InterviewVenueLookupResponse ToLookupResponse(this InterviewVenue entity)
        {
            return new InterviewVenueLookupResponse
            {
                InterviewVenueId = entity.InterviewVenueId,
                VenueName = entity.VenueName,
            };
        }
    }
}
