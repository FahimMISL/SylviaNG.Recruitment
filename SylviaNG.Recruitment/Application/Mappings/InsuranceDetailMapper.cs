using SylviaNG.Recruitment.Application.Features.InsuranceDetails.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class InsuranceDetailMapper
    {
        public static InsuranceDetail ToEntity(this InsuranceDetailCreateRequest request)
        {
            return new InsuranceDetail
            {
                PreBoardingProfileId = request.PreBoardingProfileId,
                InsuranceType = request.InsuranceType,
                ProviderName = request.ProviderName,
                PolicyNumber = request.PolicyNumber,
                BeneficiaryName = request.BeneficiaryName,
                BeneficiaryRelationship = request.BeneficiaryRelationship,
                DocumentFileUrl = request.DocumentFileUrl,
            };
        }

        public static void ApplyUpdate(this InsuranceDetail entity, InsuranceDetailUpdateRequest request)
        {
            if (request.PreBoardingProfileId.HasValue) entity.PreBoardingProfileId = request.PreBoardingProfileId.Value;
            if (request.InsuranceType is not null) entity.InsuranceType = request.InsuranceType;
            if (request.ProviderName is not null) entity.ProviderName = request.ProviderName;
            if (request.PolicyNumber is not null) entity.PolicyNumber = request.PolicyNumber;
            if (request.BeneficiaryName is not null) entity.BeneficiaryName = request.BeneficiaryName;
            if (request.BeneficiaryRelationship is not null) entity.BeneficiaryRelationship = request.BeneficiaryRelationship;
            if (request.DocumentFileUrl is not null) entity.DocumentFileUrl = request.DocumentFileUrl;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static InsuranceDetailResponse ToResponse(this InsuranceDetail entity)
        {
            return new InsuranceDetailResponse
            {
                InsuranceDetailId = entity.InsuranceDetailId,
                PreBoardingProfileId = entity.PreBoardingProfileId,
                InsuranceType = entity.InsuranceType,
                ProviderName = entity.ProviderName,
                PolicyNumber = entity.PolicyNumber,
                BeneficiaryName = entity.BeneficiaryName,
                BeneficiaryRelationship = entity.BeneficiaryRelationship,
                DocumentFileUrl = entity.DocumentFileUrl,
                IsActive = entity.IsActive,
            };
        }
    }
}
