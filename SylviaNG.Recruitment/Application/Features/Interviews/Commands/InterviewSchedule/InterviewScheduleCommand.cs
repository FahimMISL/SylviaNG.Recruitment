using MediatR;
using SylviaNG.Recruitment.Application.Features.Interviews.Models;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewSchedule
{
    public class InterviewScheduleCommand : IRequest<long>
    {
        public InterviewScheduleRequest Request { get; set; }

        public InterviewScheduleCommand(InterviewScheduleRequest request)
        {
            Request = request;
        }
    }
}
