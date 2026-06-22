using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AssessmentStages.Commands.AssessmentStageCreate
{
    public class AssessmentStageCreateHandler : IRequestHandler<AssessmentStageCreateCommand, long>
    {
        private readonly IAssessmentStageService _service;

        public AssessmentStageCreateHandler(IAssessmentStageService service)
        {
            _service = service;
        }

        public async Task<long> Handle(AssessmentStageCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
