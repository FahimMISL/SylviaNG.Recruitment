using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Commands.AssessmentWorkflowSetActive
{
    public class AssessmentWorkflowSetActiveHandler : IRequestHandler<AssessmentWorkflowSetActiveCommand, Unit>
    {
        private readonly IAssessmentWorkflowService _assessmentWorkflowService;

        public AssessmentWorkflowSetActiveHandler(IAssessmentWorkflowService assessmentWorkflowService)
        {
            _assessmentWorkflowService = assessmentWorkflowService;
        }

        public async Task<Unit> Handle(AssessmentWorkflowSetActiveCommand command, CancellationToken cancellationToken)
        {
            await _assessmentWorkflowService.SetActiveAsync(command.AssessmentWorkflowId, command.IsActive);
            return Unit.Value;
        }
    }
}
