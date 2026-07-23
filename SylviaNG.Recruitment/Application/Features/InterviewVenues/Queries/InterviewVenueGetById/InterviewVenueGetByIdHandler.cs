using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Queries.InterviewVenueGetById
{
    public class InterviewVenueGetByIdHandler : IRequestHandler<InterviewVenueGetByIdQuery, InterviewVenueResponse>
    {
        private readonly IInterviewVenueService _interviewVenueService;

        public InterviewVenueGetByIdHandler(IInterviewVenueService interviewVenueService)
        {
            _interviewVenueService = interviewVenueService;
        }

        public async Task<InterviewVenueResponse> Handle(InterviewVenueGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _interviewVenueService.GetByIdAsync(query.InterviewVenueId);
        }
    }
}
