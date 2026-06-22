using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.QuestionOptions.Commands.QuestionOptionDelete
{
    public class QuestionOptionDeleteHandler : IRequestHandler<QuestionOptionDeleteCommand, Unit>
    {
        private readonly IQuestionOptionService _service;

        public QuestionOptionDeleteHandler(IQuestionOptionService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(QuestionOptionDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.QuestionOptionId);
            return Unit.Value;
        }
    }
}
