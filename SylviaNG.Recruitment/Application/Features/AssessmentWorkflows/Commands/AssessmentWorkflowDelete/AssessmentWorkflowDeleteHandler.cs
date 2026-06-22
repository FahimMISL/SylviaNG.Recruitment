using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Commands.AssessmentWorkflowDelete
{
    public class AssessmentWorkflowDeleteHandler : IRequestHandler<AssessmentWorkflowDeleteCommand, Unit>
    {
        private readonly IAssessmentWorkflowService _service;

        public AssessmentWorkflowDeleteHandler(IAssessmentWorkflowService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(AssessmentWorkflowDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.AssessmentWorkflowId);
            return Unit.Value;
        }
    }
}
