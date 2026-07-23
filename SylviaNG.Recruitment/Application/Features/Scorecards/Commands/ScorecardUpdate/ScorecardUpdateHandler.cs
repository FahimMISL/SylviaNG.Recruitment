using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Scorecards.Commands.ScorecardUpdate
{
    public class ScorecardUpdateHandler : IRequestHandler<ScorecardUpdateCommand>
    {
        private readonly IScorecardService _scorecardService;

        public ScorecardUpdateHandler(IScorecardService scorecardService)
        {
            _scorecardService = scorecardService;
        }

        public async Task Handle(ScorecardUpdateCommand command, CancellationToken cancellationToken)
        {
            await _scorecardService.UpdateAsync(command.ScorecardId, command.Request);
        }
    }
}
