using MediatR;
using SylviaNG.Recruitment.Application.Features.Interviews.Models;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewCancel
{
    public class InterviewCancelCommand : IRequest
    {
        public long InterviewId { get; set; }
        public InterviewCancelRequest Request { get; set; }

        public InterviewCancelCommand(long interviewId, InterviewCancelRequest request)
        {
            InterviewId = interviewId;
            Request = request;
        }
    }
}
