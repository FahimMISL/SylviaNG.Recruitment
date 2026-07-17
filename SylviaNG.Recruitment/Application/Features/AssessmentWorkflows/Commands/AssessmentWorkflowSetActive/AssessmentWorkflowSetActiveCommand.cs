using MediatR;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Commands.AssessmentWorkflowSetActive
{
    public class AssessmentWorkflowSetActiveCommand : IRequest<Unit>
    {
        public long AssessmentWorkflowId { get; set; }
        public bool IsActive { get; set; }

        public AssessmentWorkflowSetActiveCommand(long assessmentWorkflowId, bool isActive)
        {
            AssessmentWorkflowId = assessmentWorkflowId;
            IsActive = isActive;
        }
    }
}
