using MediatR;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecards.Commands.InterviewScorecardDelete
{
    public class InterviewScorecardDeleteCommand : IRequest<Unit>
    {
        public long InterviewScorecardId { get; set; }

        public InterviewScorecardDeleteCommand(long interviewScorecardId)
        {
            InterviewScorecardId = interviewScorecardId;
        }
    }
}
