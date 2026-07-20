using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamQuestions.Commands.ExamQuestionUpdate
{
    public class ExamQuestionUpdateHandler : IRequestHandler<ExamQuestionUpdateCommand, Unit>
    {
        private readonly IExamQuestionService _examQuestionService;

        public ExamQuestionUpdateHandler(IExamQuestionService examQuestionService)
        {
            _examQuestionService = examQuestionService;
        }

        public async Task<Unit> Handle(ExamQuestionUpdateCommand command, CancellationToken cancellationToken)
        {
            await _examQuestionService.UpdateAsync(command.ExamQuestionId, command.Request);
            return Unit.Value;
        }
    }
}
