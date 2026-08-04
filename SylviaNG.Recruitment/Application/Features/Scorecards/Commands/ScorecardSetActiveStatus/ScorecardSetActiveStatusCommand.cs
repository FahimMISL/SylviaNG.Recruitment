using MediatR;

namespace SylviaNG.Recruitment.Application.Features.Scorecards.Commands.ScorecardSetActiveStatus
{
    public class ScorecardSetActiveStatusCommand : IRequest
    {
        public long ScorecardId { get; set; }
        public bool IsActive { get; set; }

        public ScorecardSetActiveStatusCommand(long scorecardId, bool isActive)
        {
            ScorecardId = scorecardId;
            IsActive = isActive;
        }
    }
}
