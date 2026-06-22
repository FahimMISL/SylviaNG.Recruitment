using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Commands.ExamHallDelete
{
    public class ExamHallDeleteHandler : IRequestHandler<ExamHallDeleteCommand, Unit>
    {
        private readonly IExamHallService _service;

        public ExamHallDeleteHandler(IExamHallService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ExamHallDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.ExamHallId);
            return Unit.Value;
        }
    }
}
