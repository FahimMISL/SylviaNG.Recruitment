using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewBulkCancel
{
    public class InterviewBulkCancelHandler : IRequestHandler<InterviewBulkCancelCommand>
    {
        private readonly IInterviewService _interviewService;

        public InterviewBulkCancelHandler(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        public async Task Handle(InterviewBulkCancelCommand command, CancellationToken cancellationToken)
        {
            await _interviewService.BulkCancelAsync(command.Request);
        }
    }
}
