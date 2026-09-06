using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Commands.QuestionGroupCreate
{
    public class QuestionGroupCreateHandler : IRequestHandler<QuestionGroupCreateCommand, long>
    {
        private readonly IQuestionGroupService _questionGroupService;

        public QuestionGroupCreateHandler(IQuestionGroupService questionGroupService)
        {
            _questionGroupService = questionGroupService;
        }

        public async Task<long> Handle(QuestionGroupCreateCommand command, CancellationToken cancellationToken)
        {
            return await _questionGroupService.CreateAsync(command.Request);
        }
    }
}
