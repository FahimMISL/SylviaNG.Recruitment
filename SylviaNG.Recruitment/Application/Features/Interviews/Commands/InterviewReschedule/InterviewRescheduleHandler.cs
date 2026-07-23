using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewReschedule
{
    public class InterviewRescheduleHandler : IRequestHandler<InterviewRescheduleCommand>
    {
        private readonly IInterviewService _interviewService;

        public InterviewRescheduleHandler(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        public async Task Handle(InterviewRescheduleCommand command, CancellationToken cancellationToken)
        {
            await _interviewService.RescheduleAsync(command.InterviewId, command.Request);
        }
    }
}
