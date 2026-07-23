using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewCancel
{
    public class InterviewCancelHandler : IRequestHandler<InterviewCancelCommand>
    {
        private readonly IInterviewService _interviewService;

        public InterviewCancelHandler(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        public async Task Handle(InterviewCancelCommand command, CancellationToken cancellationToken)
        {
            await _interviewService.CancelAsync(command.InterviewId, command.Request);
        }
    }
}
