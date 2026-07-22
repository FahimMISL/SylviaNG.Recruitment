using MediatR;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Queries.AssessmentWorkflowGetActiveLookup
{
    public class AssessmentWorkflowGetActiveLookupQuery : IRequest<List<AssessmentWorkflowLookupResponse>>
    {
    }
}
