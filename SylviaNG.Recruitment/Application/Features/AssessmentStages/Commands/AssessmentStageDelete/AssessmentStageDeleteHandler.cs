using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AssessmentStages.Commands.AssessmentStageDelete
{
    public class AssessmentStageDeleteHandler : IRequestHandler<AssessmentStageDeleteCommand, Unit>
    {
        private readonly IAssessmentStageService _service;

        public AssessmentStageDeleteHandler(IAssessmentStageService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(AssessmentStageDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.AssessmentStageId);
            return Unit.Value;
        }
    }
}
