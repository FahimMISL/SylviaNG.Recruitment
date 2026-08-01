using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewRooms.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewRooms.Commands.InterviewRoomCreate
{
    public class InterviewRoomCreateCommand : IRequest<long>
    {
        public long InterviewVenueId { get; set; }
        public InterviewRoomCreateRequest Request { get; set; }

        public InterviewRoomCreateCommand(long interviewVenueId, InterviewRoomCreateRequest request)
        {
            InterviewVenueId = interviewVenueId;
            Request = request;
        }
    }
}
