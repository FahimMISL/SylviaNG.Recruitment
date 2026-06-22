using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AssessmentStages.Commands.AssessmentStageUpdate
{
    public class AssessmentStageUpdateHandler : IRequestHandler<AssessmentStageUpdateCommand, Unit>
    {
        private readonly IAssessmentStageService _service;

        public AssessmentStageUpdateHandler(IAssessmentStageService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(AssessmentStageUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.AssessmentStageId, command.Request);
            return Unit.Value;
        }
    }
}
