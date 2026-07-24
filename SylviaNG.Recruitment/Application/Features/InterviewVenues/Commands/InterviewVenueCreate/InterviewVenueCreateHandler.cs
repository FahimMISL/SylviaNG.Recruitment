using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Commands.InterviewVenueCreate
{
    public class InterviewVenueCreateHandler : IRequestHandler<InterviewVenueCreateCommand, long>
    {
        private readonly IInterviewVenueService _interviewVenueService;

        public InterviewVenueCreateHandler(IInterviewVenueService interviewVenueService)
        {
            _interviewVenueService = interviewVenueService;
        }

        public async Task<long> Handle(InterviewVenueCreateCommand command, CancellationToken cancellationToken)
        {
            return await _interviewVenueService.CreateAsync(command.Request);
        }
    }
}
