using MediatR;
using SylviaNG.Recruitment.Application.Features.Interviews.Models;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewReschedule
{
    public class InterviewRescheduleCommand : IRequest
    {
        public long InterviewId { get; set; }
        public InterviewRescheduleRequest Request { get; set; }

        public InterviewRescheduleCommand(long interviewId, InterviewRescheduleRequest request)
        {
            InterviewId = interviewId;
            Request = request;
        }
    }
}
