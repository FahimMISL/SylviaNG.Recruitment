using SylviaNG.Recruitment.Application.Features.PreBoarding.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class PreBoardingMapper
    {
        public static PreBoardingSubmissionResponse ToResponse(this PreBoardingSubmission entity)
        {
            return new PreBoardingSubmissionResponse
            {
                PreBoardingSubmissionId = entity.PreBoardingSubmissionId,
                FinalSelectionPoolId = entity.FinalSelectionPoolId,
                Status = entity.Status,
                EmergencyContactName = entity.EmergencyContactName,
                EmergencyContactRelationship = entity.EmergencyContactRelationship,
                EmergencyContactPhone = entity.EmergencyContactPhone,
                InsuranceProvider = entity.InsuranceProvider,
                InsurancePolicyNumber = entity.InsurancePolicyNumber,
                InsuranceNotes = entity.InsuranceNotes,
                BankName = entity.BankName,
                BankBranch = entity.BankBranch,
                BankAccountName = entity.BankAccountName,
                BankAccountNumber = entity.BankAccountNumber,
                BankRoutingNumber = entity.BankRoutingNumber,
                SubmittedAt = entity.SubmittedAt,
                Nominees = entity.Nominees.Select(n => n.ToResponse()).ToList(),
            };
        }

        public static PreBoardingNomineeResponse ToResponse(this PreBoardingNominee entity)
        {
            return new PreBoardingNomineeResponse
            {
                PreBoardingNomineeId = entity.PreBoardingNomineeId,
                FullName = entity.FullName,
                Relationship = entity.Relationship,
                SharePercentage = entity.SharePercentage,
                ContactPhone = entity.ContactPhone,
                Address = entity.Address,
            };
        }
    }
}
