using SylviaNG.Recruitment.Application.Features.HiringPipelines.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    /// <summary>
    /// Manual mapping methods for HiringPipeline and PipelineStage entities.
    /// </summary>
    public static class HiringPipelineMapper
    {
        public static PipelineStage ToEntity(this PipelineStageRequest request)
        {
            return new PipelineStage
            {
                Name = request.Name,
                StageType = request.StageType,
                DisplayOrder = request.DisplayOrder,
                Description = request.Description,
                PassingCriteria = request.PassingCriteria,
                IsActive = true,
                IsMandatory = request.IsMandatory,
                EstimatedDurationMinutes = request.EstimatedDurationMinutes,
                SlaDays = request.SlaDays,
                MaxMarks = request.MaxMarks,
                PassMarks = request.PassMarks,
                RequiredDocuments = request.RequiredDocuments,
                AutoProgressionRule = request.AutoProgressionRule,
                AutoProgressionTargetDisplayOrder = request.AutoProgressionTargetDisplayOrder,
                ManualApprovalRequired = request.ManualApprovalRequired
            };
        }

        public static PipelineStageResponse ToResponse(this PipelineStage entity)
        {
            return new PipelineStageResponse
            {
                PipelineStageId = entity.PipelineStageId,
                Name = entity.Name,
                StageType = entity.StageType,
                DisplayOrder = entity.DisplayOrder,
                Description = entity.Description,
                PassingCriteria = entity.PassingCriteria,
                IsActive = entity.IsActive,
                IsMandatory = entity.IsMandatory,
                EstimatedDurationMinutes = entity.EstimatedDurationMinutes,
                SlaDays = entity.SlaDays,
                MaxMarks = entity.MaxMarks,
                PassMarks = entity.PassMarks,
                RequiredDocuments = entity.RequiredDocuments,
                AutoProgressionRule = entity.AutoProgressionRule,
                AutoProgressionTargetDisplayOrder = entity.AutoProgressionTargetDisplayOrder,
                ManualApprovalRequired = entity.ManualApprovalRequired
            };
        }

        public static HiringPipeline ToEntity(this HiringPipelineCreateRequest request)
        {
            return new HiringPipeline
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = true,
                Stages = request.Stages.OrderBy(s => s.DisplayOrder).Select(s => s.ToEntity()).ToList()
            };
        }

        public static HiringPipelineResponse ToResponse(this HiringPipeline entity)
        {
            return new HiringPipelineResponse
            {
                HiringPipelineId = entity.HiringPipelineId,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = entity.IsActive,
                JobPostingCount = entity.JobPostings?.Count ?? 0,
                Stages = entity.Stages?
                    .OrderBy(s => s.DisplayOrder)
                    .Select(s => s.ToResponse())
                    .ToList() ?? new List<PipelineStageResponse>()
            };
        }

        public static HiringPipelineLookupResponse ToLookupResponse(this HiringPipeline entity)
        {
            return new HiringPipelineLookupResponse
            {
                HiringPipelineId = entity.HiringPipelineId,
                Name = entity.Name
            };
        }
    }
}
