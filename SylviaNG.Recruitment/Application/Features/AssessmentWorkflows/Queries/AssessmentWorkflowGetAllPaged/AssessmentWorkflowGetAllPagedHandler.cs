using MediatR;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Queries.AssessmentWorkflowGetAllPaged
{
    public class AssessmentWorkflowGetAllPagedHandler : IRequestHandler<AssessmentWorkflowGetAllPagedQuery, PagedResult<AssessmentWorkflowResponse>>
    {
        private readonly IAssessmentWorkflowService _assessmentWorkflowService;

        public AssessmentWorkflowGetAllPagedHandler(IAssessmentWorkflowService assessmentWorkflowService)
        {
            _assessmentWorkflowService = assessmentWorkflowService;
        }

        public async Task<PagedResult<AssessmentWorkflowResponse>> Handle(AssessmentWorkflowGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _assessmentWorkflowService.GetPaginatedAsync(query.Request);
        }
    }
}
