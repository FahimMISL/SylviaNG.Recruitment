using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Commands.ExamHallUpdate
{
    public class ExamHallUpdateHandler : IRequestHandler<ExamHallUpdateCommand, Unit>
    {
        private readonly IExamHallService _service;

        public ExamHallUpdateHandler(IExamHallService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ExamHallUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.ExamHallId, command.Request);
            return Unit.Value;
        }
    }
}
