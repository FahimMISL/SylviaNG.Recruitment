using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.QuestionOptions.Commands.QuestionOptionUpdate
{
    public class QuestionOptionUpdateHandler : IRequestHandler<QuestionOptionUpdateCommand, Unit>
    {
        private readonly IQuestionOptionService _service;

        public QuestionOptionUpdateHandler(IQuestionOptionService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(QuestionOptionUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.QuestionOptionId, command.Request);
            return Unit.Value;
        }
    }
}
