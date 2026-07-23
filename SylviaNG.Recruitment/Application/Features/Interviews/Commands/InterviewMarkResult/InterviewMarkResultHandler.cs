using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewMarkResult
{
    public class InterviewMarkResultHandler : IRequestHandler<InterviewMarkResultCommand>
    {
        private readonly IInterviewService _interviewService;

        public InterviewMarkResultHandler(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        public async Task Handle(InterviewMarkResultCommand command, CancellationToken cancellationToken)
        {
            await _interviewService.MarkResultAsync(command.InterviewId, command.Request);
        }
    }
}
