using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class AssessmentWorkflowMapper
    {
        public static AssessmentWorkflow ToEntity(this AssessmentWorkflowCreateRequest request)
        {
            return new AssessmentWorkflow
            {
                RequisitionId = request.RequisitionId,
                WorkflowName = request.WorkflowName,
            };
        }

        public static void ApplyUpdate(this AssessmentWorkflow entity, AssessmentWorkflowUpdateRequest request)
        {
            if (request.RequisitionId.HasValue) entity.RequisitionId = request.RequisitionId.Value;
            if (request.WorkflowName is not null) entity.WorkflowName = request.WorkflowName;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static AssessmentWorkflowResponse ToResponse(this AssessmentWorkflow entity)
        {
            return new AssessmentWorkflowResponse
            {
                AssessmentWorkflowId = entity.AssessmentWorkflowId,
                RequisitionId = entity.RequisitionId,
                WorkflowName = entity.WorkflowName,
                IsActive = entity.IsActive,
            };
        }
    }
}
