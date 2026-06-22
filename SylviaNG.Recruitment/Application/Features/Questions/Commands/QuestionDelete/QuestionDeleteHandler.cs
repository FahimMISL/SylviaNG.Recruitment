using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Questions.Commands.QuestionDelete
{
    public class QuestionDeleteHandler : IRequestHandler<QuestionDeleteCommand, Unit>
    {
        private readonly IQuestionService _service;

        public QuestionDeleteHandler(IQuestionService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(QuestionDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.QuestionId);
            return Unit.Value;
        }
    }
}
