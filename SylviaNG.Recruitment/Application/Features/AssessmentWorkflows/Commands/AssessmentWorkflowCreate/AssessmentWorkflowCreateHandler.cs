using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Commands.AssessmentWorkflowCreate
{
    public class AssessmentWorkflowCreateHandler : IRequestHandler<AssessmentWorkflowCreateCommand, long>
    {
        private readonly IAssessmentWorkflowService _assessmentWorkflowService;

        public AssessmentWorkflowCreateHandler(IAssessmentWorkflowService assessmentWorkflowService)
        {
            _assessmentWorkflowService = assessmentWorkflowService;
        }

        public async Task<long> Handle(AssessmentWorkflowCreateCommand command, CancellationToken cancellationToken)
        {
            return await _assessmentWorkflowService.CreateAsync(command.Request);
        }
    }
}
