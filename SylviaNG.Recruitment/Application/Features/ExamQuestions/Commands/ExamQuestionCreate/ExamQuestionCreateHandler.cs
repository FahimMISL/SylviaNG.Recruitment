using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamQuestions.Commands.ExamQuestionCreate
{
    public class ExamQuestionCreateHandler : IRequestHandler<ExamQuestionCreateCommand, long>
    {
        private readonly IExamQuestionService _examQuestionService;

        public ExamQuestionCreateHandler(IExamQuestionService examQuestionService)
        {
            _examQuestionService = examQuestionService;
        }

        public async Task<long> Handle(ExamQuestionCreateCommand command, CancellationToken cancellationToken)
        {
            return await _examQuestionService.CreateAsync(command.Request);
        }
    }
}
