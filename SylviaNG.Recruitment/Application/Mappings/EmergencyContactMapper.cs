using SylviaNG.Recruitment.Application.Features.EmergencyContacts.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class EmergencyContactMapper
    {
        public static EmergencyContact ToEntity(this EmergencyContactCreateRequest request)
        {
            return new EmergencyContact
            {
                PreBoardingProfileId = request.PreBoardingProfileId,
                ContactName = request.ContactName,
                Relationship = request.Relationship,
                Phone = request.Phone,
                AlternatePhone = request.AlternatePhone,
                Address = request.Address,
                IsPrimary = request.IsPrimary,
            };
        }

        public static void ApplyUpdate(this EmergencyContact entity, EmergencyContactUpdateRequest request)
        {
            if (request.PreBoardingProfileId.HasValue) entity.PreBoardingProfileId = request.PreBoardingProfileId.Value;
            if (request.ContactName is not null) entity.ContactName = request.ContactName;
            if (request.Relationship is not null) entity.Relationship = request.Relationship;
            if (request.Phone is not null) entity.Phone = request.Phone;
            if (request.AlternatePhone is not null) entity.AlternatePhone = request.AlternatePhone;
            if (request.Address is not null) entity.Address = request.Address;
            if (request.IsPrimary.HasValue) entity.IsPrimary = request.IsPrimary.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static EmergencyContactResponse ToResponse(this EmergencyContact entity)
        {
            return new EmergencyContactResponse
            {
                EmergencyContactId = entity.EmergencyContactId,
                PreBoardingProfileId = entity.PreBoardingProfileId,
                ContactName = entity.ContactName,
                Relationship = entity.Relationship,
                Phone = entity.Phone,
                AlternatePhone = entity.AlternatePhone,
                Address = entity.Address,
                IsPrimary = entity.IsPrimary,
                IsActive = entity.IsActive,
            };
        }
    }
}
