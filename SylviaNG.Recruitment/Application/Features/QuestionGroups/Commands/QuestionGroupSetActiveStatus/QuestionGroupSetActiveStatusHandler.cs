using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Commands.QuestionGroupSetActiveStatus
{
    public class QuestionGroupSetActiveStatusHandler : IRequestHandler<QuestionGroupSetActiveStatusCommand, Unit>
    {
        private readonly IQuestionGroupService _questionGroupService;

        public QuestionGroupSetActiveStatusHandler(IQuestionGroupService questionGroupService)
        {
            _questionGroupService = questionGroupService;
        }

        public async Task<Unit> Handle(QuestionGroupSetActiveStatusCommand command, CancellationToken cancellationToken)
        {
            await _questionGroupService.SetActiveStatusAsync(command.QuestionGroupId, command.IsActive);
            return Unit.Value;
        }
    }
}
