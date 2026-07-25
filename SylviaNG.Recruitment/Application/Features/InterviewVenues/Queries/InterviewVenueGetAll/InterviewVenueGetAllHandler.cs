using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Queries.InterviewVenueGetAll
{
    public class InterviewVenueGetAllHandler : IRequestHandler<InterviewVenueGetAllQuery, List<InterviewVenueResponse>>
    {
        private readonly IInterviewVenueService _interviewVenueService;

        public InterviewVenueGetAllHandler(IInterviewVenueService interviewVenueService)
        {
            _interviewVenueService = interviewVenueService;
        }

        public async Task<List<InterviewVenueResponse>> Handle(InterviewVenueGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _interviewVenueService.GetAllAsync();
        }
    }
}
