using MediatR;

namespace SylviaNG.Recruitment.Application.Features.InterviewSessions.Commands.InterviewSessionDelete
{
    public class InterviewSessionDeleteCommand : IRequest<Unit>
    {
        public long InterviewSessionId { get; set; }

        public InterviewSessionDeleteCommand(long interviewSessionId)
        {
            InterviewSessionId = interviewSessionId;
        }
    }
}
