using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecards.Commands.InterviewScorecardDelete
{
    public class InterviewScorecardDeleteHandler : IRequestHandler<InterviewScorecardDeleteCommand, Unit>
    {
        private readonly IInterviewScorecardService _service;

        public InterviewScorecardDeleteHandler(IInterviewScorecardService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(InterviewScorecardDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.InterviewScorecardId);
            return Unit.Value;
        }
    }
}
