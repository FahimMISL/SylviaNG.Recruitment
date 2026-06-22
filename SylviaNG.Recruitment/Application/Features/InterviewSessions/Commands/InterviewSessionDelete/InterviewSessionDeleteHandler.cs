using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewSessions.Commands.InterviewSessionDelete
{
    public class InterviewSessionDeleteHandler : IRequestHandler<InterviewSessionDeleteCommand, Unit>
    {
        private readonly IInterviewSessionService _service;

        public InterviewSessionDeleteHandler(IInterviewSessionService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(InterviewSessionDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.InterviewSessionId);
            return Unit.Value;
        }
    }
}
