using SylviaNG.Recruitment.Application.Features.Referrals.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class ReferralMapper
    {
        public static Referral ToEntity(this ReferralCreateRequest request)
        {
            return new Referral
            {
                CandidateId = request.CandidateId,
                JobPostingId = request.JobPostingId,
                Source = request.Source,
                ReferrerEmployeeId = request.ReferrerEmployeeId,
                RecruitmentAgencyId = request.RecruitmentAgencyId,
                ReferrerName = request.ReferrerName,
                ReferrerContact = request.ReferrerContact,
                ReferralStatus = request.ReferralStatus,
            };
        }

        public static void ApplyUpdate(this Referral entity, ReferralUpdateRequest request)
        {
            if (request.CandidateId.HasValue) entity.CandidateId = request.CandidateId.Value;
            if (request.JobPostingId.HasValue) entity.JobPostingId = request.JobPostingId.Value;
            if (request.Source.HasValue) entity.Source = request.Source.Value;
            if (request.ReferrerEmployeeId.HasValue) entity.ReferrerEmployeeId = request.ReferrerEmployeeId.Value;
            if (request.RecruitmentAgencyId.HasValue) entity.RecruitmentAgencyId = request.RecruitmentAgencyId.Value;
            if (request.ReferrerName is not null) entity.ReferrerName = request.ReferrerName;
            if (request.ReferrerContact is not null) entity.ReferrerContact = request.ReferrerContact;
            if (request.ReferralStatus.HasValue) entity.ReferralStatus = request.ReferralStatus.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static ReferralResponse ToResponse(this Referral entity)
        {
            return new ReferralResponse
            {
                ReferralId = entity.ReferralId,
                CandidateId = entity.CandidateId,
                JobPostingId = entity.JobPostingId,
                Source = entity.Source,
                ReferrerEmployeeId = entity.ReferrerEmployeeId,
                RecruitmentAgencyId = entity.RecruitmentAgencyId,
                ReferrerName = entity.ReferrerName,
                ReferrerContact = entity.ReferrerContact,
                ReferralStatus = entity.ReferralStatus,
                IsActive = entity.IsActive,
            };
        }
    }
}
