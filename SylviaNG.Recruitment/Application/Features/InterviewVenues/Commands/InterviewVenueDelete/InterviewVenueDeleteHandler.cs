using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Commands.InterviewVenueDelete
{
    public class InterviewVenueDeleteHandler : IRequestHandler<InterviewVenueDeleteCommand, Unit>
    {
        private readonly IInterviewVenueService _service;

        public InterviewVenueDeleteHandler(IInterviewVenueService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(InterviewVenueDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.InterviewVenueId);
            return Unit.Value;
        }
    }
}
