using MediatR;
using SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Queries.VerificationWorkflowGetAllPaged
{
    public class VerificationWorkflowGetAllPagedHandler : IRequestHandler<VerificationWorkflowGetAllPagedQuery, PagedResult<VerificationWorkflowResponse>>
    {
        private readonly IVerificationWorkflowService _verificationWorkflowService;

        public VerificationWorkflowGetAllPagedHandler(IVerificationWorkflowService verificationWorkflowService)
        {
            _verificationWorkflowService = verificationWorkflowService;
        }

        public async Task<PagedResult<VerificationWorkflowResponse>> Handle(VerificationWorkflowGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _verificationWorkflowService.GetPaginatedAsync(query.Request);
        }
    }
}
