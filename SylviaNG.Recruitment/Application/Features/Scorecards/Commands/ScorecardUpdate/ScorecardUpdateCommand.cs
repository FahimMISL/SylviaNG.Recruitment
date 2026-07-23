using MediatR;
using SylviaNG.Recruitment.Application.Features.Scorecards.Models;

namespace SylviaNG.Recruitment.Application.Features.Scorecards.Commands.ScorecardUpdate
{
    public class ScorecardUpdateCommand : IRequest
    {
        public long ScorecardId { get; set; }
        public ScorecardUpdateRequest Request { get; set; }

        public ScorecardUpdateCommand(long scorecardId, ScorecardUpdateRequest request)
        {
            ScorecardId = scorecardId;
            Request = request;
        }
    }
}
