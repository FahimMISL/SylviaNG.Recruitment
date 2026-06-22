using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.QuestionOptions.Commands.QuestionOptionCreate
{
    public class QuestionOptionCreateHandler : IRequestHandler<QuestionOptionCreateCommand, long>
    {
        private readonly IQuestionOptionService _service;

        public QuestionOptionCreateHandler(IQuestionOptionService service)
        {
            _service = service;
        }

        public async Task<long> Handle(QuestionOptionCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
