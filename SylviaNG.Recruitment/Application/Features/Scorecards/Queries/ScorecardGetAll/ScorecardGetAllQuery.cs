using MediatR;
using SylviaNG.Recruitment.Application.Features.Scorecards.Models;

namespace SylviaNG.Recruitment.Application.Features.Scorecards.Queries.ScorecardGetAll
{
    public class ScorecardGetAllQuery : IRequest<List<ScorecardResponse>>
    {
    }
}
