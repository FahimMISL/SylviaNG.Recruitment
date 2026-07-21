using MediatR;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Queries.AssessmentWorkflowGetById
{
    public class AssessmentWorkflowGetByIdQuery : IRequest<AssessmentWorkflowResponse>
    {
        public long AssessmentWorkflowId { get; set; }

        public AssessmentWorkflowGetByIdQuery(long assessmentWorkflowId)
        {
            AssessmentWorkflowId = assessmentWorkflowId;
        }
    }
}
