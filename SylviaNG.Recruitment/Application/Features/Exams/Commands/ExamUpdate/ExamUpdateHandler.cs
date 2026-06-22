using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Exams.Commands.ExamUpdate
{
    public class ExamUpdateHandler : IRequestHandler<ExamUpdateCommand, Unit>
    {
        private readonly IExamService _service;

        public ExamUpdateHandler(IExamService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ExamUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.ExamId, command.Request);
            return Unit.Value;
        }
    }
}
