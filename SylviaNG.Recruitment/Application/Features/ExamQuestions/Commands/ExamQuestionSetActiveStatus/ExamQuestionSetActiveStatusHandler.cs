using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamQuestions.Commands.ExamQuestionSetActiveStatus
{
    public class ExamQuestionSetActiveStatusHandler : IRequestHandler<ExamQuestionSetActiveStatusCommand, Unit>
    {
        private readonly IExamQuestionService _examQuestionService;

        public ExamQuestionSetActiveStatusHandler(IExamQuestionService examQuestionService)
        {
            _examQuestionService = examQuestionService;
        }

        public async Task<Unit> Handle(ExamQuestionSetActiveStatusCommand command, CancellationToken cancellationToken)
        {
            await _examQuestionService.SetActiveStatusAsync(command.ExamQuestionId, command.IsActive);
            return Unit.Value;
        }
    }
}
