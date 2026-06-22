using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Commands.QuestionGroupDelete
{
    public class QuestionGroupDeleteHandler : IRequestHandler<QuestionGroupDeleteCommand, Unit>
    {
        private readonly IQuestionGroupService _service;

        public QuestionGroupDeleteHandler(IQuestionGroupService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(QuestionGroupDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.QuestionGroupId);
            return Unit.Value;
        }
    }
}
