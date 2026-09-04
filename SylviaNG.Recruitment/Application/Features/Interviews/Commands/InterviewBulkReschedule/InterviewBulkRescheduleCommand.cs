using MediatR;
using SylviaNG.Recruitment.Application.Features.Interviews.Models;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewBulkReschedule
{
    public class InterviewBulkRescheduleCommand : IRequest
    {
        public InterviewBulkRescheduleRequest Request { get; set; }

        public InterviewBulkRescheduleCommand(InterviewBulkRescheduleRequest request)
        {
            Request = request;
        }
    }
}
