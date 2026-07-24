using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewRooms.Commands.InterviewRoomSetActiveStatus
{
    public class InterviewRoomSetActiveStatusHandler : IRequestHandler<InterviewRoomSetActiveStatusCommand>
    {
        private readonly IInterviewRoomService _interviewRoomService;

        public InterviewRoomSetActiveStatusHandler(IInterviewRoomService interviewRoomService)
        {
            _interviewRoomService = interviewRoomService;
        }

        public async Task Handle(InterviewRoomSetActiveStatusCommand command, CancellationToken cancellationToken)
        {
            await _interviewRoomService.SetActiveStatusAsync(command.InterviewRoomId, command.IsActive);
        }
    }
}
