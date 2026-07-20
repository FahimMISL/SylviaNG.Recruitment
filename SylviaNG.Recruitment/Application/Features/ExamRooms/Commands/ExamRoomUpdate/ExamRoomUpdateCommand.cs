using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamRooms.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamRooms.Commands.ExamRoomUpdate
{
    public class ExamRoomUpdateCommand : IRequest
    {
        public long ExamRoomId { get; set; }
        public ExamRoomUpdateRequest Request { get; set; }

        public ExamRoomUpdateCommand(long examRoomId, ExamRoomUpdateRequest request)
        {
            ExamRoomId = examRoomId;
            Request = request;
        }
    }
}
