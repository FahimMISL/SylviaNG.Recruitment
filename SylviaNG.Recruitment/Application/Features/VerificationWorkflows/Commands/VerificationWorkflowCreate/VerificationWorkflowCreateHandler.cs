using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Commands.VerificationWorkflowCreate
{
    public class VerificationWorkflowCreateHandler : IRequestHandler<VerificationWorkflowCreateCommand, long>
    {
        private readonly IVerificationWorkflowService _service;

        public VerificationWorkflowCreateHandler(IVerificationWorkflowService service)
        {
            _service = service;
        }

        public async Task<long> Handle(VerificationWorkflowCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
