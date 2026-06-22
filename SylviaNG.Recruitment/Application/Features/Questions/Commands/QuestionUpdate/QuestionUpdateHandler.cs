using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Questions.Commands.QuestionUpdate
{
    public class QuestionUpdateHandler : IRequestHandler<QuestionUpdateCommand, Unit>
    {
        private readonly IQuestionService _service;

        public QuestionUpdateHandler(IQuestionService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(QuestionUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.QuestionId, command.Request);
            return Unit.Value;
        }
    }
}
