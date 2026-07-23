using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewBulkSchedule
{
    public class InterviewBulkScheduleHandler : IRequestHandler<InterviewBulkScheduleCommand, List<long>>
    {
        private readonly IInterviewService _interviewService;

        public InterviewBulkScheduleHandler(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        public async Task<List<long>> Handle(InterviewBulkScheduleCommand command, CancellationToken cancellationToken)
        {
            return await _interviewService.BulkScheduleAsync(command.Request);
        }
    }
}
