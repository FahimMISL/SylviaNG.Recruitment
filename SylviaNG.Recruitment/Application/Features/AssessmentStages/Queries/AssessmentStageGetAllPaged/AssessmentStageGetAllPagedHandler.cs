using MediatR;
using SylviaNG.Recruitment.Application.Features.AssessmentStages.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.AssessmentStages.Queries.AssessmentStageGetAllPaged
{
    public class AssessmentStageGetAllPagedHandler : IRequestHandler<AssessmentStageGetAllPagedQuery, PagedResult<AssessmentStageResponse>>
    {
        private readonly IAssessmentStageService _assessmentStageService;

        public AssessmentStageGetAllPagedHandler(IAssessmentStageService assessmentStageService)
        {
            _assessmentStageService = assessmentStageService;
        }

        public async Task<PagedResult<AssessmentStageResponse>> Handle(AssessmentStageGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _assessmentStageService.GetPaginatedAsync(query.Request);
        }
    }
}
