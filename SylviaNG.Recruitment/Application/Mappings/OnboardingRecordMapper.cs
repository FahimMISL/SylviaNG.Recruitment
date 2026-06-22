using SylviaNG.Recruitment.Application.Features.OnboardingRecords.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class OnboardingRecordMapper
    {
        public static OnboardingRecord ToEntity(this OnboardingRecordCreateRequest request)
        {
            return new OnboardingRecord
            {
                CandidateId = request.CandidateId,
                JobApplicationId = request.JobApplicationId,
                Stage = request.Stage,
                CoreHrEmployeeId = request.CoreHrEmployeeId,
                PayrollReferenceId = request.PayrollReferenceId,
                PreHireSentAt = request.PreHireSentAt,
                PostHireSentAt = request.PostHireSentAt,
                IsPreHireSuccess = request.IsPreHireSuccess,
                IsPostHireSuccess = request.IsPostHireSuccess,
                FailureDetails = request.FailureDetails,
                RetryCount = request.RetryCount,
            };
        }

        public static void ApplyUpdate(this OnboardingRecord entity, OnboardingRecordUpdateRequest request)
        {
            if (request.CandidateId.HasValue) entity.CandidateId = request.CandidateId.Value;
            if (request.JobApplicationId.HasValue) entity.JobApplicationId = request.JobApplicationId.Value;
            if (request.Stage.HasValue) entity.Stage = request.Stage.Value;
            if (request.CoreHrEmployeeId is not null) entity.CoreHrEmployeeId = request.CoreHrEmployeeId;
            if (request.PayrollReferenceId is not null) entity.PayrollReferenceId = request.PayrollReferenceId;
            if (request.PreHireSentAt.HasValue) entity.PreHireSentAt = request.PreHireSentAt;
            if (request.PostHireSentAt.HasValue) entity.PostHireSentAt = request.PostHireSentAt;
            if (request.IsPreHireSuccess.HasValue) entity.IsPreHireSuccess = request.IsPreHireSuccess.Value;
            if (request.IsPostHireSuccess.HasValue) entity.IsPostHireSuccess = request.IsPostHireSuccess.Value;
            if (request.FailureDetails is not null) entity.FailureDetails = request.FailureDetails;
            if (request.RetryCount.HasValue) entity.RetryCount = request.RetryCount.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static OnboardingRecordResponse ToResponse(this OnboardingRecord entity)
        {
            return new OnboardingRecordResponse
            {
                OnboardingRecordId = entity.OnboardingRecordId,
                CandidateId = entity.CandidateId,
                JobApplicationId = entity.JobApplicationId,
                Stage = entity.Stage,
                CoreHrEmployeeId = entity.CoreHrEmployeeId,
                PayrollReferenceId = entity.PayrollReferenceId,
                PreHireSentAt = entity.PreHireSentAt,
                PostHireSentAt = entity.PostHireSentAt,
                IsPreHireSuccess = entity.IsPreHireSuccess,
                IsPostHireSuccess = entity.IsPostHireSuccess,
                FailureDetails = entity.FailureDetails,
                RetryCount = entity.RetryCount,
                IsActive = entity.IsActive,
            };
        }
    }
}
