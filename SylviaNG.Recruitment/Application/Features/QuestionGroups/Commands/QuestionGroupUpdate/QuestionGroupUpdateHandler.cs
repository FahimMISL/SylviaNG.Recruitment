using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Commands.QuestionGroupUpdate
{
    public class QuestionGroupUpdateHandler : IRequestHandler<QuestionGroupUpdateCommand, Unit>
    {
        private readonly IQuestionGroupService _service;

        public QuestionGroupUpdateHandler(IQuestionGroupService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(QuestionGroupUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.QuestionGroupId, command.Request);
            return Unit.Value;
        }
    }
}
