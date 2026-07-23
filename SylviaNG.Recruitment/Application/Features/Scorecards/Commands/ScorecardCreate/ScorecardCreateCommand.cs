using MediatR;
using SylviaNG.Recruitment.Application.Features.Scorecards.Models;

namespace SylviaNG.Recruitment.Application.Features.Scorecards.Commands.ScorecardCreate
{
    public class ScorecardCreateCommand : IRequest<long>
    {
        public ScorecardCreateRequest Request { get; set; }

        public ScorecardCreateCommand(ScorecardCreateRequest request)
        {
            Request = request;
        }
    }
}
