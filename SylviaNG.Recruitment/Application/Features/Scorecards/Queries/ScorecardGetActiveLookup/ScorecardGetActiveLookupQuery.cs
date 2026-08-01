using MediatR;
using SylviaNG.Recruitment.Application.Features.Scorecards.Models;

namespace SylviaNG.Recruitment.Application.Features.Scorecards.Queries.ScorecardGetActiveLookup
{
    public class ScorecardGetActiveLookupQuery : IRequest<List<ScorecardLookupResponse>>
    {
    }
}
