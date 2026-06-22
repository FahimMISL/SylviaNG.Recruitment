using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewSessions.Commands.InterviewSessionCreate
{
    public class InterviewSessionCreateHandler : IRequestHandler<InterviewSessionCreateCommand, long>
    {
        private readonly IInterviewSessionService _service;

        public InterviewSessionCreateHandler(IInterviewSessionService service)
        {
            _service = service;
        }

        public async Task<long> Handle(InterviewSessionCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
