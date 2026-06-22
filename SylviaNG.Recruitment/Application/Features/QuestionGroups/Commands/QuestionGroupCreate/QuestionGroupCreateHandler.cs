using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Commands.QuestionGroupCreate
{
    public class QuestionGroupCreateHandler : IRequestHandler<QuestionGroupCreateCommand, long>
    {
        private readonly IQuestionGroupService _service;

        public QuestionGroupCreateHandler(IQuestionGroupService service)
        {
            _service = service;
        }

        public async Task<long> Handle(QuestionGroupCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
