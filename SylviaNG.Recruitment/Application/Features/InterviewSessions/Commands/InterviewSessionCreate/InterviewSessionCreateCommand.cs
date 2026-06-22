using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewSessions.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewSessions.Commands.InterviewSessionCreate
{
    public class InterviewSessionCreateCommand : IRequest<long>
    {
        public InterviewSessionCreateRequest Request { get; set; }

        public InterviewSessionCreateCommand(InterviewSessionCreateRequest request)
        {
            Request = request;
        }
    }
}
