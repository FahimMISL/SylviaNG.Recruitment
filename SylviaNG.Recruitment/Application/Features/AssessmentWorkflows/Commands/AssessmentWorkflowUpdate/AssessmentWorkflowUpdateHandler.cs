using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Commands.AssessmentWorkflowUpdate
{
    public class AssessmentWorkflowUpdateHandler : IRequestHandler<AssessmentWorkflowUpdateCommand, Unit>
    {
        private readonly IAssessmentWorkflowService _assessmentWorkflowService;

        public AssessmentWorkflowUpdateHandler(IAssessmentWorkflowService assessmentWorkflowService)
        {
            _assessmentWorkflowService = assessmentWorkflowService;
        }

        public async Task<Unit> Handle(AssessmentWorkflowUpdateCommand command, CancellationToken cancellationToken)
        {
            await _assessmentWorkflowService.UpdateAsync(command.AssessmentWorkflowId, command.Request);
            return Unit.Value;
        }
    }
}
