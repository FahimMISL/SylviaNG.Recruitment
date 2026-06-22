using MediatR;
using SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Queries.VerificationWorkflowGetAllPaged
{
    public class VerificationWorkflowGetAllPagedQuery : IRequest<PagedResult<VerificationWorkflowResponse>>
    {
        public PagedRequest Request { get; set; }

        public VerificationWorkflowGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
