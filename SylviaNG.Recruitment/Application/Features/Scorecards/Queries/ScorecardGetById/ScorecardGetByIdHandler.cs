using MediatR;
using SylviaNG.Recruitment.Application.Features.Scorecards.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Scorecards.Queries.ScorecardGetById
{
    public class ScorecardGetByIdHandler : IRequestHandler<ScorecardGetByIdQuery, ScorecardResponse>
    {
        private readonly IScorecardService _scorecardService;

        public ScorecardGetByIdHandler(IScorecardService scorecardService)
        {
            _scorecardService = scorecardService;
        }

        public async Task<ScorecardResponse> Handle(ScorecardGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _scorecardService.GetByIdAsync(query.ScorecardId);
        }
    }
}
