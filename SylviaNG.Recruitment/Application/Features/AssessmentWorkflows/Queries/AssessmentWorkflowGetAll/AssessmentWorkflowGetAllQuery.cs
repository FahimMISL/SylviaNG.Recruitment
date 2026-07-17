using MediatR;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Queries.AssessmentWorkflowGetAll
{
    public class AssessmentWorkflowGetAllQuery : IRequest<List<AssessmentWorkflowResponse>>
    {
    }
}
