using MediatR;
using SylviaNG.Recruitment.Application.Features.Interviews.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Queries.InterviewGetAllPaged
{
    public class InterviewGetAllPagedHandler : IRequestHandler<InterviewGetAllPagedQuery, PagedResult<InterviewResponse>>
    {
        private readonly IInterviewService _interviewService;

        public InterviewGetAllPagedHandler(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        public async Task<PagedResult<InterviewResponse>> Handle(InterviewGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _interviewService.GetPagedAsync(query.Request, query.JobPostingId, query.Status, query.DateFrom, query.DateTo);
        }
    }
}
