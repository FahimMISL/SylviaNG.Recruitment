using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Commands.InterviewVenueUpdate
{
    public class InterviewVenueUpdateHandler : IRequestHandler<InterviewVenueUpdateCommand>
    {
        private readonly IInterviewVenueService _interviewVenueService;

        public InterviewVenueUpdateHandler(IInterviewVenueService interviewVenueService)
        {
            _interviewVenueService = interviewVenueService;
        }

        public async Task Handle(InterviewVenueUpdateCommand command, CancellationToken cancellationToken)
        {
            await _interviewVenueService.UpdateAsync(command.InterviewVenueId, command.Request);
        }
    }
}
