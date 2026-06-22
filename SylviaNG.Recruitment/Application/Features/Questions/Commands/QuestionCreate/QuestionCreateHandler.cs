using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Questions.Commands.QuestionCreate
{
    public class QuestionCreateHandler : IRequestHandler<QuestionCreateCommand, long>
    {
        private readonly IQuestionService _service;

        public QuestionCreateHandler(IQuestionService service)
        {
            _service = service;
        }

        public async Task<long> Handle(QuestionCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
