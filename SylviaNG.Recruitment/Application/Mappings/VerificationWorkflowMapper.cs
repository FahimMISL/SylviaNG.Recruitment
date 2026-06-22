using SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class VerificationWorkflowMapper
    {
        public static VerificationWorkflow ToEntity(this VerificationWorkflowCreateRequest request)
        {
            return new VerificationWorkflow
            {
                CandidateId = request.CandidateId,
                JobApplicationId = request.JobApplicationId,
                OverallStatus = request.OverallStatus,
                InitiatedByUserId = request.InitiatedByUserId,
                InitiatedAt = request.InitiatedAt,
                CompletedAt = request.CompletedAt,
            };
        }

        public static void ApplyUpdate(this VerificationWorkflow entity, VerificationWorkflowUpdateRequest request)
        {
            if (request.CandidateId.HasValue) entity.CandidateId = request.CandidateId.Value;
            if (request.JobApplicationId.HasValue) entity.JobApplicationId = request.JobApplicationId.Value;
            if (request.OverallStatus.HasValue) entity.OverallStatus = request.OverallStatus.Value;
            if (request.InitiatedByUserId.HasValue) entity.InitiatedByUserId = request.InitiatedByUserId.Value;
            if (request.InitiatedAt.HasValue) entity.InitiatedAt = request.InitiatedAt.Value;
            if (request.CompletedAt.HasValue) entity.CompletedAt = request.CompletedAt;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static VerificationWorkflowResponse ToResponse(this VerificationWorkflow entity)
        {
            return new VerificationWorkflowResponse
            {
                VerificationWorkflowId = entity.VerificationWorkflowId,
                CandidateId = entity.CandidateId,
                JobApplicationId = entity.JobApplicationId,
                OverallStatus = entity.OverallStatus,
                InitiatedByUserId = entity.InitiatedByUserId,
                InitiatedAt = entity.InitiatedAt,
                CompletedAt = entity.CompletedAt,
                IsActive = entity.IsActive,
            };
        }
    }
}
