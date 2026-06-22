using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewSessions.Commands.InterviewSessionUpdate
{
    public class InterviewSessionUpdateHandler : IRequestHandler<InterviewSessionUpdateCommand, Unit>
    {
        private readonly IInterviewSessionService _service;

        public InterviewSessionUpdateHandler(IInterviewSessionService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(InterviewSessionUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.InterviewSessionId, command.Request);
            return Unit.Value;
        }
    }
}
