using MediatR;

namespace SylviaNG.Recruitment.Application.Features.InterviewRooms.Commands.InterviewRoomSetActiveStatus
{
    public class InterviewRoomSetActiveStatusCommand : IRequest
    {
        public long InterviewRoomId { get; set; }
        public bool IsActive { get; set; }

        public InterviewRoomSetActiveStatusCommand(long interviewRoomId, bool isActive)
        {
            InterviewRoomId = interviewRoomId;
            IsActive = isActive;
        }
    }
}
