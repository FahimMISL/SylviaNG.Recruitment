using MediatR;
using SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Models;

namespace SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Commands.VerificationWorkflowUpdate
{
    public class VerificationWorkflowUpdateCommand : IRequest<Unit>
    {
        public long VerificationWorkflowId { get; set; }
        public VerificationWorkflowUpdateRequest Request { get; set; }

        public VerificationWorkflowUpdateCommand(long verificationWorkflowId, VerificationWorkflowUpdateRequest request)
        {
            VerificationWorkflowId = verificationWorkflowId;
            Request = request;
        }
    }
}
