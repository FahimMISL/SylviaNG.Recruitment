using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewSchedule
{
    public class InterviewScheduleHandler : IRequestHandler<InterviewScheduleCommand, long>
    {
        private readonly IInterviewService _interviewService;

        public InterviewScheduleHandler(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        public async Task<long> Handle(InterviewScheduleCommand command, CancellationToken cancellationToken)
        {
            return await _interviewService.ScheduleAsync(command.Request);
        }
    }
}
