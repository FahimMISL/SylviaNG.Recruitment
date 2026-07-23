using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewRooms.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewRooms.Queries.InterviewRoomGetAllByVenue
{
    public class InterviewRoomGetAllByVenueQuery : IRequest<List<InterviewRoomResponse>>
    {
        public long InterviewVenueId { get; set; }

        public InterviewRoomGetAllByVenueQuery(long interviewVenueId)
        {
            InterviewVenueId = interviewVenueId;
        }
    }
}
