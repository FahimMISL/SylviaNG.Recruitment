using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Queries.InterviewVenueGetAllPaged
{
    public class InterviewVenueGetAllPagedHandler : IRequestHandler<InterviewVenueGetAllPagedQuery, PagedResult<InterviewVenueResponse>>
    {
        private readonly IInterviewVenueService _interviewVenueService;

        public InterviewVenueGetAllPagedHandler(IInterviewVenueService interviewVenueService)
        {
            _interviewVenueService = interviewVenueService;
        }

        public async Task<PagedResult<InterviewVenueResponse>> Handle(InterviewVenueGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _interviewVenueService.GetPaginatedAsync(query.Request);
        }
    }
}
