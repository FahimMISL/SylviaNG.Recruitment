using SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class RecruitmentAgencyMapper
    {
        public static RecruitmentAgency ToEntity(this RecruitmentAgencyCreateRequest request)
        {
            return new RecruitmentAgency
            {
                AgencyName = request.AgencyName,
                ContactPerson = request.ContactPerson,
                Email = request.Email,
                Phone = request.Phone,
                AgreementDetails = request.AgreementDetails,
                AgreementStartDate = request.AgreementStartDate,
                AgreementEndDate = request.AgreementEndDate,
            };
        }

        public static void ApplyUpdate(this RecruitmentAgency entity, RecruitmentAgencyUpdateRequest request)
        {
            if (request.AgencyName is not null) entity.AgencyName = request.AgencyName;
            if (request.ContactPerson is not null) entity.ContactPerson = request.ContactPerson;
            if (request.Email is not null) entity.Email = request.Email;
            if (request.Phone is not null) entity.Phone = request.Phone;
            if (request.AgreementDetails is not null) entity.AgreementDetails = request.AgreementDetails;
            if (request.AgreementStartDate.HasValue) entity.AgreementStartDate = request.AgreementStartDate;
            if (request.AgreementEndDate.HasValue) entity.AgreementEndDate = request.AgreementEndDate;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static RecruitmentAgencyResponse ToResponse(this RecruitmentAgency entity)
        {
            return new RecruitmentAgencyResponse
            {
                RecruitmentAgencyId = entity.RecruitmentAgencyId,
                AgencyName = entity.AgencyName,
                ContactPerson = entity.ContactPerson,
                Email = entity.Email,
                Phone = entity.Phone,
                AgreementDetails = entity.AgreementDetails,
                AgreementStartDate = entity.AgreementStartDate,
                AgreementEndDate = entity.AgreementEndDate,
                IsActive = entity.IsActive,
            };
        }
    }
}
