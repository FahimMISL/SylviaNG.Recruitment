using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewSessions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.InterviewSessions.Queries.InterviewSessionGetAllPaged
{
    public class InterviewSessionGetAllPagedHandler : IRequestHandler<InterviewSessionGetAllPagedQuery, PagedResult<InterviewSessionResponse>>
    {
        private readonly IInterviewSessionService _interviewSessionService;

        public InterviewSessionGetAllPagedHandler(IInterviewSessionService interviewSessionService)
        {
            _interviewSessionService = interviewSessionService;
        }

        public async Task<PagedResult<InterviewSessionResponse>> Handle(InterviewSessionGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _interviewSessionService.GetPaginatedAsync(query.Request);
        }
    }
}
