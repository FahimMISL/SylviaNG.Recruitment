using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Scorecards.Commands.ScorecardSetActiveStatus
{
    public class ScorecardSetActiveStatusHandler : IRequestHandler<ScorecardSetActiveStatusCommand>
    {
        private readonly IScorecardService _scorecardService;

        public ScorecardSetActiveStatusHandler(IScorecardService scorecardService)
        {
            _scorecardService = scorecardService;
        }

        public async Task Handle(ScorecardSetActiveStatusCommand command, CancellationToken cancellationToken)
        {
            await _scorecardService.SetActiveStatusAsync(command.ScorecardId, command.IsActive);
        }
    }
}
