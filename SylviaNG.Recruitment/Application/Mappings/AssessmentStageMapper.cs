using SylviaNG.Recruitment.Application.Features.AssessmentStages.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class AssessmentStageMapper
    {
        public static AssessmentStage ToEntity(this AssessmentStageCreateRequest request)
        {
            return new AssessmentStage
            {
                AssessmentWorkflowId = request.AssessmentWorkflowId,
                AssessmentType = request.AssessmentType,
                StageName = request.StageName,
                StageOrder = request.StageOrder,
                PassThreshold = request.PassThreshold,
                Weight = request.Weight,
                IsPassFail = request.IsPassFail,
            };
        }

        public static void ApplyUpdate(this AssessmentStage entity, AssessmentStageUpdateRequest request)
        {
            if (request.AssessmentWorkflowId.HasValue) entity.AssessmentWorkflowId = request.AssessmentWorkflowId.Value;
            if (request.AssessmentType.HasValue) entity.AssessmentType = request.AssessmentType.Value;
            if (request.StageName is not null) entity.StageName = request.StageName;
            if (request.StageOrder.HasValue) entity.StageOrder = request.StageOrder.Value;
            if (request.PassThreshold.HasValue) entity.PassThreshold = request.PassThreshold.Value;
            if (request.Weight.HasValue) entity.Weight = request.Weight.Value;
            if (request.IsPassFail.HasValue) entity.IsPassFail = request.IsPassFail.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static AssessmentStageResponse ToResponse(this AssessmentStage entity)
        {
            return new AssessmentStageResponse
            {
                AssessmentStageId = entity.AssessmentStageId,
                AssessmentWorkflowId = entity.AssessmentWorkflowId,
                AssessmentType = entity.AssessmentType,
                StageName = entity.StageName,
                StageOrder = entity.StageOrder,
                PassThreshold = entity.PassThreshold,
                Weight = entity.Weight,
                IsPassFail = entity.IsPassFail,
                IsActive = entity.IsActive,
            };
        }
    }
}
