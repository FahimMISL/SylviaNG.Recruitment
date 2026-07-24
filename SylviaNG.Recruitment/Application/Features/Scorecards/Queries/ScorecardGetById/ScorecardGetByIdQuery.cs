using MediatR;
using SylviaNG.Recruitment.Application.Features.Scorecards.Models;

namespace SylviaNG.Recruitment.Application.Features.Scorecards.Queries.ScorecardGetById
{
    public class ScorecardGetByIdQuery : IRequest<ScorecardResponse>
    {
        public long ScorecardId { get; set; }

        public ScorecardGetByIdQuery(long scorecardId)
        {
            ScorecardId = scorecardId;
        }
    }
}
