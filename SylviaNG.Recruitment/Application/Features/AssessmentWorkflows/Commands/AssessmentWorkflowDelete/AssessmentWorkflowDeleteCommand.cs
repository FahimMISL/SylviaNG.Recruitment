using MediatR;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Commands.AssessmentWorkflowDelete
{
    public class AssessmentWorkflowDeleteCommand : IRequest<Unit>
    {
        public long AssessmentWorkflowId { get; set; }

        public AssessmentWorkflowDeleteCommand(long assessmentWorkflowId)
        {
            AssessmentWorkflowId = assessmentWorkflowId;
        }
    }
}
