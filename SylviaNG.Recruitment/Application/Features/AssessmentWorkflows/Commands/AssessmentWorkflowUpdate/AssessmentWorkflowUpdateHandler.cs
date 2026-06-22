using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Commands.AssessmentWorkflowUpdate
{
    public class AssessmentWorkflowUpdateHandler : IRequestHandler<AssessmentWorkflowUpdateCommand, Unit>
    {
        private readonly IAssessmentWorkflowService _service;

        public AssessmentWorkflowUpdateHandler(IAssessmentWorkflowService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(AssessmentWorkflowUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.AssessmentWorkflowId, command.Request);
            return Unit.Value;
        }
    }
}
