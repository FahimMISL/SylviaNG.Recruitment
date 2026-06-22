using MediatR;
using SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Models;

namespace SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Queries.VerificationWorkflowGetById
{
    public class VerificationWorkflowGetByIdQuery : IRequest<VerificationWorkflowResponse>
    {
        public long VerificationWorkflowId { get; set; }

        public VerificationWorkflowGetByIdQuery(long verificationWorkflowId)
        {
            VerificationWorkflowId = verificationWorkflowId;
        }
    }
}
