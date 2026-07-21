using MediatR;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Queries.AssessmentWorkflowGetActiveLookup
{
    public class AssessmentWorkflowGetActiveLookupHandler : IRequestHandler<AssessmentWorkflowGetActiveLookupQuery, List<AssessmentWorkflowLookupResponse>>
    {
        private readonly IAssessmentWorkflowService _assessmentWorkflowService;

        public AssessmentWorkflowGetActiveLookupHandler(IAssessmentWorkflowService assessmentWorkflowService)
        {
            _assessmentWorkflowService = assessmentWorkflowService;
        }

        public async Task<List<AssessmentWorkflowLookupResponse>> Handle(AssessmentWorkflowGetActiveLookupQuery query, CancellationToken cancellationToken)
        {
            return await _assessmentWorkflowService.GetActiveLookupAsync();
        }
    }
}
