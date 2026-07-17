using MediatR;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Queries.AssessmentWorkflowGetAll
{
    public class AssessmentWorkflowGetAllHandler : IRequestHandler<AssessmentWorkflowGetAllQuery, List<AssessmentWorkflowResponse>>
    {
        private readonly IAssessmentWorkflowService _assessmentWorkflowService;

        public AssessmentWorkflowGetAllHandler(IAssessmentWorkflowService assessmentWorkflowService)
        {
            _assessmentWorkflowService = assessmentWorkflowService;
        }

        public async Task<List<AssessmentWorkflowResponse>> Handle(AssessmentWorkflowGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _assessmentWorkflowService.GetAllAsync();
        }
    }
}
