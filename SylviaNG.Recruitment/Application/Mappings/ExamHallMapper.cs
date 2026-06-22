using SylviaNG.Recruitment.Application.Features.ExamHalls.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class ExamHallMapper
    {
        public static ExamHall ToEntity(this ExamHallCreateRequest request)
        {
            return new ExamHall
            {
                VenueName = request.VenueName,
                Address = request.Address,
                VirtualLink = request.VirtualLink,
                Capacity = request.Capacity,
                NumberOfRooms = request.NumberOfRooms,
                ContactPerson = request.ContactPerson,
                ContactPhone = request.ContactPhone,
            };
        }

        public static void ApplyUpdate(this ExamHall entity, ExamHallUpdateRequest request)
        {
            if (request.VenueName is not null) entity.VenueName = request.VenueName;
            if (request.Address is not null) entity.Address = request.Address;
            if (request.VirtualLink is not null) entity.VirtualLink = request.VirtualLink;
            if (request.Capacity.HasValue) entity.Capacity = request.Capacity.Value;
            if (request.NumberOfRooms.HasValue) entity.NumberOfRooms = request.NumberOfRooms.Value;
            if (request.ContactPerson is not null) entity.ContactPerson = request.ContactPerson;
            if (request.ContactPhone is not null) entity.ContactPhone = request.ContactPhone;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static ExamHallResponse ToResponse(this ExamHall entity)
        {
            return new ExamHallResponse
            {
                ExamHallId = entity.ExamHallId,
                VenueName = entity.VenueName,
                Address = entity.Address,
                VirtualLink = entity.VirtualLink,
                Capacity = entity.Capacity,
                NumberOfRooms = entity.NumberOfRooms,
                ContactPerson = entity.ContactPerson,
                ContactPhone = entity.ContactPhone,
                IsActive = entity.IsActive,
            };
        }
    }
}
