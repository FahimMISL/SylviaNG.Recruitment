using MediatR;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Commands.AssessmentWorkflowUpdate
{
    public class AssessmentWorkflowUpdateCommand : IRequest<Unit>
    {
        public long AssessmentWorkflowId { get; set; }
        public AssessmentWorkflowUpdateRequest Request { get; set; }

        public AssessmentWorkflowUpdateCommand(long assessmentWorkflowId, AssessmentWorkflowUpdateRequest request)
        {
            AssessmentWorkflowId = assessmentWorkflowId;
            Request = request;
        }
    }
}
