using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Commands.VerificationWorkflowUpdate
{
    public class VerificationWorkflowUpdateHandler : IRequestHandler<VerificationWorkflowUpdateCommand, Unit>
    {
        private readonly IVerificationWorkflowService _service;

        public VerificationWorkflowUpdateHandler(IVerificationWorkflowService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(VerificationWorkflowUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.VerificationWorkflowId, command.Request);
            return Unit.Value;
        }
    }
}
