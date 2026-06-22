using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Commands.VerificationWorkflowDelete
{
    public class VerificationWorkflowDeleteHandler : IRequestHandler<VerificationWorkflowDeleteCommand, Unit>
    {
        private readonly IVerificationWorkflowService _service;

        public VerificationWorkflowDeleteHandler(IVerificationWorkflowService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(VerificationWorkflowDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.VerificationWorkflowId);
            return Unit.Value;
        }
    }
}
