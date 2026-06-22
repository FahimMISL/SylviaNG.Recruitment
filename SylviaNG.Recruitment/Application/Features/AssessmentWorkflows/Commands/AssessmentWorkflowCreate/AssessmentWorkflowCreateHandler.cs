using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Commands.AssessmentWorkflowCreate
{
    public class AssessmentWorkflowCreateHandler : IRequestHandler<AssessmentWorkflowCreateCommand, long>
    {
        private readonly IAssessmentWorkflowService _service;

        public AssessmentWorkflowCreateHandler(IAssessmentWorkflowService service)
        {
            _service = service;
        }

        public async Task<long> Handle(AssessmentWorkflowCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
