using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamRooms.Commands.ExamRoomSetActiveStatus
{
    public class ExamRoomSetActiveStatusHandler : IRequestHandler<ExamRoomSetActiveStatusCommand>
    {
        private readonly IExamRoomService _examRoomService;

        public ExamRoomSetActiveStatusHandler(IExamRoomService examRoomService)
        {
            _examRoomService = examRoomService;
        }

        public async Task Handle(ExamRoomSetActiveStatusCommand command, CancellationToken cancellationToken)
        {
            await _examRoomService.SetActiveStatusAsync(command.ExamRoomId, command.IsActive);
        }
    }
}
