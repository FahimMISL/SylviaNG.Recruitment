using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Commands.QuestionGroupUpdate
{
    public class QuestionGroupUpdateHandler : IRequestHandler<QuestionGroupUpdateCommand, Unit>
    {
        private readonly IQuestionGroupService _questionGroupService;

        public QuestionGroupUpdateHandler(IQuestionGroupService questionGroupService)
        {
            _questionGroupService = questionGroupService;
        }

        public async Task<Unit> Handle(QuestionGroupUpdateCommand command, CancellationToken cancellationToken)
        {
            await _questionGroupService.UpdateAsync(command.QuestionGroupId, command.Request);
            return Unit.Value;
        }
    }
}
