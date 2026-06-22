using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Commands.InterviewVenueUpdate
{
    public class InterviewVenueUpdateHandler : IRequestHandler<InterviewVenueUpdateCommand, Unit>
    {
        private readonly IInterviewVenueService _service;

        public InterviewVenueUpdateHandler(IInterviewVenueService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(InterviewVenueUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.InterviewVenueId, command.Request);
            return Unit.Value;
        }
    }
}
