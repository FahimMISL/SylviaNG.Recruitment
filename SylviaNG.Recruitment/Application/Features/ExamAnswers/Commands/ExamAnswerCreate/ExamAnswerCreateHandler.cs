using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamAnswers.Commands.ExamAnswerCreate
{
    public class ExamAnswerCreateHandler : IRequestHandler<ExamAnswerCreateCommand, long>
    {
        private readonly IExamAnswerService _service;

        public ExamAnswerCreateHandler(IExamAnswerService service)
        {
            _service = service;
        }

        public async Task<long> Handle(ExamAnswerCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
