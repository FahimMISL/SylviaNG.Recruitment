using MediatR;
using SylviaNG.Recruitment.Application.Features.Scorecards.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Scorecards.Queries.ScorecardGetActiveLookup
{
    public class ScorecardGetActiveLookupHandler : IRequestHandler<ScorecardGetActiveLookupQuery, List<ScorecardLookupResponse>>
    {
        private readonly IScorecardService _scorecardService;

        public ScorecardGetActiveLookupHandler(IScorecardService scorecardService)
        {
            _scorecardService = scorecardService;
        }

        public async Task<List<ScorecardLookupResponse>> Handle(ScorecardGetActiveLookupQuery query, CancellationToken cancellationToken)
        {
            return await _scorecardService.GetActiveLookupAsync();
        }
    }
}
