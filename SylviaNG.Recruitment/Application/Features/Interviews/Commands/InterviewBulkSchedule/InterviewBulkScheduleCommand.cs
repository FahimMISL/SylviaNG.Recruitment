using MediatR;
using SylviaNG.Recruitment.Application.Features.Interviews.Models;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewBulkSchedule
{
    public class InterviewBulkScheduleCommand : IRequest<List<long>>
    {
        public InterviewBulkScheduleRequest Request { get; set; }

        public InterviewBulkScheduleCommand(InterviewBulkScheduleRequest request)
        {
            Request = request;
        }
    }
}
