using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamRooms.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamRooms.Commands.ExamRoomCreate
{
    public class ExamRoomCreateCommand : IRequest<long>
    {
        public long ExamVenueId { get; set; }
        public ExamRoomCreateRequest Request { get; set; }

        public ExamRoomCreateCommand(long examVenueId, ExamRoomCreateRequest request)
        {
            ExamVenueId = examVenueId;
            Request = request;
        }
    }
}
