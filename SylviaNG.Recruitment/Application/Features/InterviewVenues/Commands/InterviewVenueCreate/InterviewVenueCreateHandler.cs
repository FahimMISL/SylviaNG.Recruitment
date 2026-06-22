using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Commands.InterviewVenueCreate
{
    public class InterviewVenueCreateHandler : IRequestHandler<InterviewVenueCreateCommand, long>
    {
        private readonly IInterviewVenueService _service;

        public InterviewVenueCreateHandler(IInterviewVenueService service)
        {
            _service = service;
        }

        public async Task<long> Handle(InterviewVenueCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
