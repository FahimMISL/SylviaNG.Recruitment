using MediatR;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Commands.AssessmentWorkflowCreate
{
    public class AssessmentWorkflowCreateCommand : IRequest<long>
    {
        public AssessmentWorkflowCreateRequest Request { get; set; }

        public AssessmentWorkflowCreateCommand(AssessmentWorkflowCreateRequest request)
        {
            Request = request;
        }
    }
}
