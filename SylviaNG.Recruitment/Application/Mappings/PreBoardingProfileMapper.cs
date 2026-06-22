using SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class PreBoardingProfileMapper
    {
        public static PreBoardingProfile ToEntity(this PreBoardingProfileCreateRequest request)
        {
            return new PreBoardingProfile
            {
                CandidateId = request.CandidateId,
                JobApplicationId = request.JobApplicationId,
                ProfileStatus = request.ProfileStatus,
                SubmittedAt = request.SubmittedAt,
                ValidatedAt = request.ValidatedAt,
                ValidatedByUserId = request.ValidatedByUserId,
                CorrectionComments = request.CorrectionComments,
                IsLocked = request.IsLocked,
            };
        }

        public static void ApplyUpdate(this PreBoardingProfile entity, PreBoardingProfileUpdateRequest request)
        {
            if (request.CandidateId.HasValue) entity.CandidateId = request.CandidateId.Value;
            if (request.JobApplicationId.HasValue) entity.JobApplicationId = request.JobApplicationId.Value;
            if (request.ProfileStatus.HasValue) entity.ProfileStatus = request.ProfileStatus.Value;
            if (request.SubmittedAt.HasValue) entity.SubmittedAt = request.SubmittedAt;
            if (request.ValidatedAt.HasValue) entity.ValidatedAt = request.ValidatedAt;
            if (request.ValidatedByUserId.HasValue) entity.ValidatedByUserId = request.ValidatedByUserId.Value;
            if (request.CorrectionComments is not null) entity.CorrectionComments = request.CorrectionComments;
            if (request.IsLocked.HasValue) entity.IsLocked = request.IsLocked.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static PreBoardingProfileResponse ToResponse(this PreBoardingProfile entity)
        {
            return new PreBoardingProfileResponse
            {
                PreBoardingProfileId = entity.PreBoardingProfileId,
                CandidateId = entity.CandidateId,
                JobApplicationId = entity.JobApplicationId,
                ProfileStatus = entity.ProfileStatus,
                SubmittedAt = entity.SubmittedAt,
                ValidatedAt = entity.ValidatedAt,
                ValidatedByUserId = entity.ValidatedByUserId,
                CorrectionComments = entity.CorrectionComments,
                IsLocked = entity.IsLocked,
                IsActive = entity.IsActive,
            };
        }
    }
}
