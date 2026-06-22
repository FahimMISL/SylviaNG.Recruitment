using MediatR;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Queries.AssessmentWorkflowGetAll
{
    public class AssessmentWorkflowGetAllHandler : IRequestHandler<AssessmentWorkflowGetAllQuery, List<AssessmentWorkflowResponse>>
    {
        private readonly IAssessmentWorkflowService _service;

        public AssessmentWorkflowGetAllHandler(IAssessmentWorkflowService service)
        {
            _service = service;
        }

        public async Task<List<AssessmentWorkflowResponse>> Handle(AssessmentWorkflowGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
