using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Queries.InterviewVenueGetActiveLookup
{
    public class InterviewVenueGetActiveLookupHandler : IRequestHandler<InterviewVenueGetActiveLookupQuery, List<InterviewVenueLookupResponse>>
    {
        private readonly IInterviewVenueService _interviewVenueService;

        public InterviewVenueGetActiveLookupHandler(IInterviewVenueService interviewVenueService)
        {
            _interviewVenueService = interviewVenueService;
        }

        public async Task<List<InterviewVenueLookupResponse>> Handle(InterviewVenueGetActiveLookupQuery query, CancellationToken cancellationToken)
        {
            return await _interviewVenueService.GetActiveLookupAsync();
        }
    }
}
