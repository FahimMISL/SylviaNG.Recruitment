using MediatR;

namespace SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Commands.VerificationWorkflowDelete
{
    public class VerificationWorkflowDeleteCommand : IRequest<Unit>
    {
        public long VerificationWorkflowId { get; set; }

        public VerificationWorkflowDeleteCommand(long verificationWorkflowId)
        {
            VerificationWorkflowId = verificationWorkflowId;
        }
    }
}
