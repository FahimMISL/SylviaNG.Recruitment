using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewRooms.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewRooms.Commands.InterviewRoomUpdate
{
    public class InterviewRoomUpdateCommand : IRequest
    {
        public long InterviewRoomId { get; set; }
        public InterviewRoomUpdateRequest Request { get; set; }

        public InterviewRoomUpdateCommand(long interviewRoomId, InterviewRoomUpdateRequest request)
        {
            InterviewRoomId = interviewRoomId;
            Request = request;
        }
    }
}
