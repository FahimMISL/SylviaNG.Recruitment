using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Queries.RequisitionApprovalGetAllPaged
{
    public class RequisitionApprovalGetAllPagedQuery : IRequest<PagedResult<RequisitionApprovalResponse>>
    {
        public PagedRequest Request { get; set; }

        public RequisitionApprovalGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
