using SylviaNG.Recruitment.Application.Features.InterviewVenues.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class InterviewVenueMapper
    {
        public static InterviewVenue ToEntity(this InterviewVenueCreateRequest request)
        {
            return new InterviewVenue
            {
                VenueName = request.VenueName,
                Address = request.Address,
                Capacity = request.Capacity,
                EquipmentDetails = request.EquipmentDetails,
                ContactPerson = request.ContactPerson,
                ContactPhone = request.ContactPhone,
            };
        }

        public static void ApplyUpdate(this InterviewVenue entity, InterviewVenueUpdateRequest request)
        {
            if (request.VenueName is not null) entity.VenueName = request.VenueName;
            if (request.Address is not null) entity.Address = request.Address;
            if (request.Capacity.HasValue) entity.Capacity = request.Capacity.Value;
            if (request.EquipmentDetails is not null) entity.EquipmentDetails = request.EquipmentDetails;
            if (request.ContactPerson is not null) entity.ContactPerson = request.ContactPerson;
            if (request.ContactPhone is not null) entity.ContactPhone = request.ContactPhone;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static InterviewVenueResponse ToResponse(this InterviewVenue entity)
        {
            return new InterviewVenueResponse
            {
                InterviewVenueId = entity.InterviewVenueId,
                VenueName = entity.VenueName,
                Address = entity.Address,
                Capacity = entity.Capacity,
                EquipmentDetails = entity.EquipmentDetails,
                ContactPerson = entity.ContactPerson,
                ContactPhone = entity.ContactPhone,
                IsActive = entity.IsActive,
            };
        }
    }
}
