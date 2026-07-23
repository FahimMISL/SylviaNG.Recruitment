using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamRooms.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamRooms.Queries.ExamRoomGetAllByVenue
{
    public class ExamRoomGetAllByVenueQuery : IRequest<List<ExamRoomResponse>>
    {
        public long ExamVenueId { get; set; }

        public ExamRoomGetAllByVenueQuery(long examVenueId)
        {
            ExamVenueId = examVenueId;
        }
    }
}
