using MediatR;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Queries.AssessmentWorkflowGetAllPaged
{
    public class AssessmentWorkflowGetAllPagedQuery : IRequest<PagedResult<AssessmentWorkflowResponse>>
    {
        public PagedRequest Request { get; set; }

        public AssessmentWorkflowGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
