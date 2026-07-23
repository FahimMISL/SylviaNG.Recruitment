using MediatR;
using SylviaNG.Recruitment.Application.Features.Scorecards.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Scorecards.Queries.ScorecardGetAll
{
    public class ScorecardGetAllHandler : IRequestHandler<ScorecardGetAllQuery, List<ScorecardResponse>>
    {
        private readonly IScorecardService _scorecardService;

        public ScorecardGetAllHandler(IScorecardService scorecardService)
        {
            _scorecardService = scorecardService;
        }

        public async Task<List<ScorecardResponse>> Handle(ScorecardGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _scorecardService.GetAllAsync();
        }
    }
}
