using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Scorecards.Commands.ScorecardCreate
{
    public class ScorecardCreateHandler : IRequestHandler<ScorecardCreateCommand, long>
    {
        private readonly IScorecardService _scorecardService;

        public ScorecardCreateHandler(IScorecardService scorecardService)
        {
            _scorecardService = scorecardService;
        }

        public async Task<long> Handle(ScorecardCreateCommand command, CancellationToken cancellationToken)
        {
            return await _scorecardService.CreateAsync(command.Request);
        }
    }
}
