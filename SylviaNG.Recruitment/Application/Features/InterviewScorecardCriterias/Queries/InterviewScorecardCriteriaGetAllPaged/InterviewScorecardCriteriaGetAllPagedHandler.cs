using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Queries.InterviewScorecardCriteriaGetAllPaged
{
    public class InterviewScorecardCriteriaGetAllPagedHandler : IRequestHandler<InterviewScorecardCriteriaGetAllPagedQuery, PagedResult<InterviewScorecardCriteriaResponse>>
    {
        private readonly IInterviewScorecardCriteriaService _interviewScorecardCriteriaService;

        public InterviewScorecardCriteriaGetAllPagedHandler(IInterviewScorecardCriteriaService interviewScorecardCriteriaService)
        {
            _interviewScorecardCriteriaService = interviewScorecardCriteriaService;
        }

        public async Task<PagedResult<InterviewScorecardCriteriaResponse>> Handle(InterviewScorecardCriteriaGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _interviewScorecardCriteriaService.GetPaginatedAsync(query.Request);
        }
    }
}
