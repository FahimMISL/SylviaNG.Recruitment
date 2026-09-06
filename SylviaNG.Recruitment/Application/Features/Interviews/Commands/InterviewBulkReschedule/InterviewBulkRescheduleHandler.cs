using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewBulkReschedule
{
    public class InterviewBulkRescheduleHandler : IRequestHandler<InterviewBulkRescheduleCommand>
    {
        private readonly IInterviewService _interviewService;

        public InterviewBulkRescheduleHandler(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        public async Task Handle(InterviewBulkRescheduleCommand command, CancellationToken cancellationToken)
        {
            await _interviewService.BulkRescheduleAsync(command.Request);
        }
    }
}
