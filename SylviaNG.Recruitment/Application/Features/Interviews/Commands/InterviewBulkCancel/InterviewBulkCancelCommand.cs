using MediatR;
using SylviaNG.Recruitment.Application.Features.Interviews.Models;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewBulkCancel
{
    public class InterviewBulkCancelCommand : IRequest
    {
        public InterviewBulkCancelRequest Request { get; set; }

        public InterviewBulkCancelCommand(InterviewBulkCancelRequest request)
        {
            Request = request;
        }
    }
}
