using SylviaNG.Recruitment.Application.Features.Nominees.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class NomineeMapper
    {
        public static Nominee ToEntity(this NomineeCreateRequest request)
        {
            return new Nominee
            {
                PreBoardingProfileId = request.PreBoardingProfileId,
                NomineeName = request.NomineeName,
                Relationship = request.Relationship,
                DateOfBirth = request.DateOfBirth,
                NationalIdNumber = request.NationalIdNumber,
                Phone = request.Phone,
                Address = request.Address,
                SharePercentage = request.SharePercentage,
                IdProofFileUrl = request.IdProofFileUrl,
            };
        }

        public static void ApplyUpdate(this Nominee entity, NomineeUpdateRequest request)
        {
            if (request.PreBoardingProfileId.HasValue) entity.PreBoardingProfileId = request.PreBoardingProfileId.Value;
            if (request.NomineeName is not null) entity.NomineeName = request.NomineeName;
            if (request.Relationship is not null) entity.Relationship = request.Relationship;
            if (request.DateOfBirth.HasValue) entity.DateOfBirth = request.DateOfBirth;
            if (request.NationalIdNumber is not null) entity.NationalIdNumber = request.NationalIdNumber;
            if (request.Phone is not null) entity.Phone = request.Phone;
            if (request.Address is not null) entity.Address = request.Address;
            if (request.SharePercentage.HasValue) entity.SharePercentage = request.SharePercentage.Value;
            if (request.IdProofFileUrl is not null) entity.IdProofFileUrl = request.IdProofFileUrl;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static NomineeResponse ToResponse(this Nominee entity)
        {
            return new NomineeResponse
            {
                NomineeId = entity.NomineeId,
                PreBoardingProfileId = entity.PreBoardingProfileId,
                NomineeName = entity.NomineeName,
                Relationship = entity.Relationship,
                DateOfBirth = entity.DateOfBirth,
                NationalIdNumber = entity.NationalIdNumber,
                Phone = entity.Phone,
                Address = entity.Address,
                SharePercentage = entity.SharePercentage,
                IdProofFileUrl = entity.IdProofFileUrl,
                IsActive = entity.IsActive,
            };
        }
    }
}
