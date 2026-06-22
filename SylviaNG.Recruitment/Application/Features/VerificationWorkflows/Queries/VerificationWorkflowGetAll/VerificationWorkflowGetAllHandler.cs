using MediatR;
using SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Queries.VerificationWorkflowGetAll
{
    public class VerificationWorkflowGetAllHandler : IRequestHandler<VerificationWorkflowGetAllQuery, List<VerificationWorkflowResponse>>
    {
        private readonly IVerificationWorkflowService _service;

        public VerificationWorkflowGetAllHandler(IVerificationWorkflowService service)
        {
            _service = service;
        }

        public async Task<List<VerificationWorkflowResponse>> Handle(VerificationWorkflowGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
