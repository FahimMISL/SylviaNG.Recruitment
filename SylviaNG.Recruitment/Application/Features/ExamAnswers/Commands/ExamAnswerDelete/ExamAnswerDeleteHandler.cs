using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamAnswers.Commands.ExamAnswerDelete
{
    public class ExamAnswerDeleteHandler : IRequestHandler<ExamAnswerDeleteCommand, Unit>
    {
        private readonly IExamAnswerService _service;

        public ExamAnswerDeleteHandler(IExamAnswerService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ExamAnswerDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.ExamAnswerId);
            return Unit.Value;
        }
    }
}
