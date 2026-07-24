using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewRooms.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewRooms.Queries.InterviewRoomGetById
{
    public class InterviewRoomGetByIdQuery : IRequest<InterviewRoomResponse>
    {
        public long InterviewRoomId { get; set; }

        public InterviewRoomGetByIdQuery(long interviewRoomId)
        {
            InterviewRoomId = interviewRoomId;
        }
    }
}
