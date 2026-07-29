using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamRooms.Commands.ExamRoomUpdate
{
    public class ExamRoomUpdateHandler : IRequestHandler<ExamRoomUpdateCommand>
    {
        private readonly IExamRoomService _examRoomService;

        public ExamRoomUpdateHandler(IExamRoomService examRoomService)
        {
            _examRoomService = examRoomService;
        }

        public async Task Handle(ExamRoomUpdateCommand command, CancellationToken cancellationToken)
        {
            await _examRoomService.UpdateAsync(command.ExamRoomId, command.Request);
        }
    }
}
