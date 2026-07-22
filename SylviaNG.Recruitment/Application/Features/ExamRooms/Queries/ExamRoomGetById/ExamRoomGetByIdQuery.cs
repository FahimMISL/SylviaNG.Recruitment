using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamRooms.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamRooms.Queries.ExamRoomGetById
{
    public class ExamRoomGetByIdQuery : IRequest<ExamRoomResponse>
    {
        public long ExamRoomId { get; set; }

        public ExamRoomGetByIdQuery(long examRoomId)
        {
            ExamRoomId = examRoomId;
        }
    }
}
