using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewScorecards.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecards.Queries.InterviewScorecardGetAllPaged
{
    public class InterviewScorecardGetAllPagedHandler : IRequestHandler<InterviewScorecardGetAllPagedQuery, PagedResult<InterviewScorecardResponse>>
    {
        private readonly IInterviewScorecardService _interviewScorecardService;

        public InterviewScorecardGetAllPagedHandler(IInterviewScorecardService interviewScorecardService)
        {
            _interviewScorecardService = interviewScorecardService;
        }

        public async Task<PagedResult<InterviewScorecardResponse>> Handle(InterviewScorecardGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _interviewScorecardService.GetPaginatedAsync(query.Request);
        }
    }
}
