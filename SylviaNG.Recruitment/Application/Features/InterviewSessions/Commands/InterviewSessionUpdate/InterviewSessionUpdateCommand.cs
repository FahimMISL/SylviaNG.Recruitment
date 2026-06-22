using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewSessions.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewSessions.Commands.InterviewSessionUpdate
{
    public class InterviewSessionUpdateCommand : IRequest<Unit>
    {
        public long InterviewSessionId { get; set; }
        public InterviewSessionUpdateRequest Request { get; set; }

        public InterviewSessionUpdateCommand(long interviewSessionId, InterviewSessionUpdateRequest request)
        {
            InterviewSessionId = interviewSessionId;
            Request = request;
        }
    }
}
