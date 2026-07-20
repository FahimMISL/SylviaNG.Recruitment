using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ExamRooms.Commands.ExamRoomSetActiveStatus
{
    public class ExamRoomSetActiveStatusCommand : IRequest
    {
        public long ExamRoomId { get; set; }
        public bool IsActive { get; set; }

        public ExamRoomSetActiveStatusCommand(long examRoomId, bool isActive)
        {
            ExamRoomId = examRoomId;
            IsActive = isActive;
        }
    }
}
