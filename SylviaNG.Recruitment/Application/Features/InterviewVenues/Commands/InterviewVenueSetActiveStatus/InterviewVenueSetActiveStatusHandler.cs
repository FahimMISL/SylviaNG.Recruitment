using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Commands.InterviewVenueSetActiveStatus
{
    public class InterviewVenueSetActiveStatusHandler : IRequestHandler<InterviewVenueSetActiveStatusCommand>
    {
        private readonly IInterviewVenueService _interviewVenueService;

        public InterviewVenueSetActiveStatusHandler(IInterviewVenueService interviewVenueService)
        {
            _interviewVenueService = interviewVenueService;
        }

        public async Task Handle(InterviewVenueSetActiveStatusCommand command, CancellationToken cancellationToken)
        {
            await _interviewVenueService.SetActiveStatusAsync(command.InterviewVenueId, command.IsActive);
        }
    }
}
