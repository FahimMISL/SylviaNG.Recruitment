using SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class HiringPipelineStageMapper
    {
        public static HiringPipelineStage ToEntity(this HiringPipelineStageCreateRequest request)
        {
            return new HiringPipelineStage
            {
                JobPostingId = request.JobPostingId,
                StageName = request.StageName,
                StageType = request.StageType,
                StageOrder = request.StageOrder,
                IsMandatory = request.IsMandatory,
                Description = request.Description,
            };
        }

        public static void ApplyUpdate(this HiringPipelineStage entity, HiringPipelineStageUpdateRequest request)
        {
            if (request.JobPostingId.HasValue) entity.JobPostingId = request.JobPostingId.Value;
            if (request.StageName is not null) entity.StageName = request.StageName;
            if (request.StageType is not null) entity.StageType = request.StageType;
            if (request.StageOrder.HasValue) entity.StageOrder = request.StageOrder.Value;
            if (request.IsMandatory.HasValue) entity.IsMandatory = request.IsMandatory.Value;
            if (request.Description is not null) entity.Description = request.Description;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static HiringPipelineStageResponse ToResponse(this HiringPipelineStage entity)
        {
            return new HiringPipelineStageResponse
            {
                HiringPipelineStageId = entity.HiringPipelineStageId,
                JobPostingId = entity.JobPostingId,
                StageName = entity.StageName,
                StageType = entity.StageType,
                StageOrder = entity.StageOrder,
                IsMandatory = entity.IsMandatory,
                Description = entity.Description,
                IsActive = entity.IsActive,
            };
        }
    }
}
