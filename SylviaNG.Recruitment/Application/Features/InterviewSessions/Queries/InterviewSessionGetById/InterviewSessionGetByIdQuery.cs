using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewSessions.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewSessions.Queries.InterviewSessionGetById
{
    public class InterviewSessionGetByIdQuery : IRequest<InterviewSessionResponse>
    {
        public long InterviewSessionId { get; set; }

        public InterviewSessionGetByIdQuery(long interviewSessionId)
        {
            InterviewSessionId = interviewSessionId;
        }
    }
}
