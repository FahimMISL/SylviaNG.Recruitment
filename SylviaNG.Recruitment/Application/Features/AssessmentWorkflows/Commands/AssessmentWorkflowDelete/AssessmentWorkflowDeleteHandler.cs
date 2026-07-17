using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Commands.AssessmentWorkflowDelete
{
    public class AssessmentWorkflowDeleteHandler : IRequestHandler<AssessmentWorkflowDeleteCommand, Unit>
    {
        private readonly IAssessmentWorkflowService _assessmentWorkflowService;

        public AssessmentWorkflowDeleteHandler(IAssessmentWorkflowService assessmentWorkflowService)
        {
            _assessmentWorkflowService = assessmentWorkflowService;
        }

        public async Task<Unit> Handle(AssessmentWorkflowDeleteCommand command, CancellationToken cancellationToken)
        {
            await _assessmentWorkflowService.DeleteAsync(command.AssessmentWorkflowId);
            return Unit.Value;
        }
    }
}
