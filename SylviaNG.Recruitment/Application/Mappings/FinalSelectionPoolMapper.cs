using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class FinalSelectionPoolMapper
    {
        public static FinalSelectionPool ToEntity(this FinalSelectionPoolCreateRequest request)
        {
            return new FinalSelectionPool
            {
                CandidateId = request.CandidateId,
                JobApplicationId = request.JobApplicationId,
                ExpectedJoiningDate = request.ExpectedJoiningDate,
                JoiningBatch = request.JoiningBatch,
                OnboardingChecklistJson = request.OnboardingChecklistJson,
                HasJoined = request.HasJoined,
                ActualJoiningDate = request.ActualJoiningDate,
            };
        }

        public static void ApplyUpdate(this FinalSelectionPool entity, FinalSelectionPoolUpdateRequest request)
        {
            if (request.CandidateId.HasValue) entity.CandidateId = request.CandidateId.Value;
            if (request.JobApplicationId.HasValue) entity.JobApplicationId = request.JobApplicationId.Value;
            if (request.ExpectedJoiningDate.HasValue) entity.ExpectedJoiningDate = request.ExpectedJoiningDate;
            if (request.JoiningBatch is not null) entity.JoiningBatch = request.JoiningBatch;
            if (request.OnboardingChecklistJson is not null) entity.OnboardingChecklistJson = request.OnboardingChecklistJson;
            if (request.HasJoined.HasValue) entity.HasJoined = request.HasJoined.Value;
            if (request.ActualJoiningDate.HasValue) entity.ActualJoiningDate = request.ActualJoiningDate;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static FinalSelectionPoolResponse ToResponse(this FinalSelectionPool entity)
        {
            return new FinalSelectionPoolResponse
            {
                FinalSelectionPoolId = entity.FinalSelectionPoolId,
                CandidateId = entity.CandidateId,
                JobApplicationId = entity.JobApplicationId,
                ExpectedJoiningDate = entity.ExpectedJoiningDate,
                JoiningBatch = entity.JoiningBatch,
                OnboardingChecklistJson = entity.OnboardingChecklistJson,
                HasJoined = entity.HasJoined,
                ActualJoiningDate = entity.ActualJoiningDate,
                IsActive = entity.IsActive,
            };
        }
    }
}
