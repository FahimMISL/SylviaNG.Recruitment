using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Exams.Commands.ExamDelete
{
    public class ExamDeleteHandler : IRequestHandler<ExamDeleteCommand, Unit>
    {
        private readonly IExamService _service;

        public ExamDeleteHandler(IExamService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ExamDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.ExamId);
            return Unit.Value;
        }
    }
}
