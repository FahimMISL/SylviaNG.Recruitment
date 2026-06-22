using MediatR;
using SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Models;

namespace SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Commands.VerificationWorkflowCreate
{
    public class VerificationWorkflowCreateCommand : IRequest<long>
    {
        public VerificationWorkflowCreateRequest Request { get; set; }

        public VerificationWorkflowCreateCommand(VerificationWorkflowCreateRequest request)
        {
            Request = request;
        }
    }
}
