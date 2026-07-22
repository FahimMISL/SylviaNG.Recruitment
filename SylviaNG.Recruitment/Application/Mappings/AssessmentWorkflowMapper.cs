using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    /// <summary>
    /// Manual mapping methods for AssessmentWorkflow and AssessmentStage entities.
    /// </summary>
    public static class AssessmentWorkflowMapper
    {
        public static AssessmentStage ToEntity(this AssessmentStageRequest request)
        {
            return new AssessmentStage
            {
                StageType = request.StageType,
                MaxMarks = request.MaxMarks,
                PassMarks = request.PassMarks,
                DurationMinutes = request.DurationMinutes,
                DisplayOrder = request.DisplayOrder,
                IsMandatory = request.IsMandatory
            };
        }

        public static AssessmentStageResponse ToResponse(this AssessmentStage entity)
        {
            return new AssessmentStageResponse
            {
                AssessmentStageId = entity.AssessmentStageId,
                StageType = entity.StageType,
                MaxMarks = entity.MaxMarks,
                PassMarks = entity.PassMarks,
                DurationMinutes = entity.DurationMinutes,
                DisplayOrder = entity.DisplayOrder,
                IsMandatory = entity.IsMandatory
            };
        }

        public static AssessmentWorkflow ToEntity(this AssessmentWorkflowCreateRequest request)
        {
            return new AssessmentWorkflow
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = true,
                Stages = request.Stages.OrderBy(s => s.DisplayOrder).Select(s => s.ToEntity()).ToList()
            };
        }

        public static AssessmentWorkflowResponse ToResponse(this AssessmentWorkflow entity)
        {
            return new AssessmentWorkflowResponse
            {
                AssessmentWorkflowId = entity.AssessmentWorkflowId,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = entity.IsActive,
                JobPostingCount = entity.JobPostings?.Count ?? 0,
                Stages = entity.Stages?
                    .OrderBy(s => s.DisplayOrder)
                    .Select(s => s.ToResponse())
                    .ToList() ?? new List<AssessmentStageResponse>()
            };
        }

        public static AssessmentWorkflowLookupResponse ToLookupResponse(this AssessmentWorkflow entity)
        {
            return new AssessmentWorkflowLookupResponse
            {
                AssessmentWorkflowId = entity.AssessmentWorkflowId,
                Name = entity.Name
            };
        }
    }
}
