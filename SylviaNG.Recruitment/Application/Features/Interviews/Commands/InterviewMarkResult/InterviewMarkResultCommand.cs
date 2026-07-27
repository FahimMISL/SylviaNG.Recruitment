using MediatR;
using SylviaNG.Recruitment.Application.Features.Interviews.Models;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewMarkResult
{
    public class InterviewMarkResultCommand : IRequest
    {
        public long InterviewId { get; set; }
        public InterviewMarkResultRequest Request { get; set; }

        public InterviewMarkResultCommand(long interviewId, InterviewMarkResultRequest request)
        {
            InterviewId = interviewId;
            Request = request;
        }
    }
}
