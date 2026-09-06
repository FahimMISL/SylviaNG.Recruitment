using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamRooms.Commands.ExamRoomCreate
{
    public class ExamRoomCreateHandler : IRequestHandler<ExamRoomCreateCommand, long>
    {
        private readonly IExamRoomService _examRoomService;

        public ExamRoomCreateHandler(IExamRoomService examRoomService)
        {
            _examRoomService = examRoomService;
        }

        public async Task<long> Handle(ExamRoomCreateCommand command, CancellationToken cancellationToken)
        {
            return await _examRoomService.CreateAsync(command.ExamVenueId, command.Request);
        }
    }
}
