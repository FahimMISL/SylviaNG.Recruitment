using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewScorecards.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecards.Commands.InterviewScorecardUpdate
{
    public class InterviewScorecardUpdateCommand : IRequest<Unit>
    {
        public long InterviewScorecardId { get; set; }
        public InterviewScorecardUpdateRequest Request { get; set; }

        public InterviewScorecardUpdateCommand(long interviewScorecardId, InterviewScorecardUpdateRequest request)
        {
            InterviewScorecardId = interviewScorecardId;
            Request = request;
        }
    }
}
