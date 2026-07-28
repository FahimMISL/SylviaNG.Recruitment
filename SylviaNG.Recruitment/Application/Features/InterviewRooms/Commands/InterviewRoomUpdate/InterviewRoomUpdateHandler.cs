using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewRooms.Commands.InterviewRoomUpdate
{
    public class InterviewRoomUpdateHandler : IRequestHandler<InterviewRoomUpdateCommand>
    {
        private readonly IInterviewRoomService _interviewRoomService;

        public InterviewRoomUpdateHandler(IInterviewRoomService interviewRoomService)
        {
            _interviewRoomService = interviewRoomService;
        }

        public async Task Handle(InterviewRoomUpdateCommand command, CancellationToken cancellationToken)
        {
            await _interviewRoomService.UpdateAsync(command.InterviewRoomId, command.Request);
        }
    }
}
