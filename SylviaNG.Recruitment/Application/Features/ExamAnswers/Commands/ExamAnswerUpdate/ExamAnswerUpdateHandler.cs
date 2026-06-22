using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamAnswers.Commands.ExamAnswerUpdate
{
    public class ExamAnswerUpdateHandler : IRequestHandler<ExamAnswerUpdateCommand, Unit>
    {
        private readonly IExamAnswerService _service;

        public ExamAnswerUpdateHandler(IExamAnswerService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ExamAnswerUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.ExamAnswerId, command.Request);
            return Unit.Value;
        }
    }
}
