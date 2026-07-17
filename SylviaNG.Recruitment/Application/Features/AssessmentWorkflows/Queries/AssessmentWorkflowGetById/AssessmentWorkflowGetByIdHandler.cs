using MediatR;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Queries.AssessmentWorkflowGetById
{
    public class AssessmentWorkflowGetByIdHandler : IRequestHandler<AssessmentWorkflowGetByIdQuery, AssessmentWorkflowResponse>
    {
        private readonly IAssessmentWorkflowService _assessmentWorkflowService;

        public AssessmentWorkflowGetByIdHandler(IAssessmentWorkflowService assessmentWorkflowService)
        {
            _assessmentWorkflowService = assessmentWorkflowService;
        }

        public async Task<AssessmentWorkflowResponse> Handle(AssessmentWorkflowGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _assessmentWorkflowService.GetByIdAsync(query.AssessmentWorkflowId);
        }
    }
}
