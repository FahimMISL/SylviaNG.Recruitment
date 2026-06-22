using MediatR;
using SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Queries.VerificationWorkflowGetById
{
    public class VerificationWorkflowGetByIdHandler : IRequestHandler<VerificationWorkflowGetByIdQuery, VerificationWorkflowResponse>
    {
        private readonly IVerificationWorkflowService _service;

        public VerificationWorkflowGetByIdHandler(IVerificationWorkflowService service)
        {
            _service = service;
        }

        public async Task<VerificationWorkflowResponse> Handle(VerificationWorkflowGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.VerificationWorkflowId);
        }
    }
}
