using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecards.Commands.InterviewScorecardUpdate
{
    public class InterviewScorecardUpdateHandler : IRequestHandler<InterviewScorecardUpdateCommand, Unit>
    {
        private readonly IInterviewScorecardService _service;

        public InterviewScorecardUpdateHandler(IInterviewScorecardService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(InterviewScorecardUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.InterviewScorecardId, command.Request);
            return Unit.Value;
        }
    }
}
