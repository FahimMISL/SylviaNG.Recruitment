using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Queries.RequisitionApprovalGetAllPaged
{
    public class RequisitionApprovalGetAllPagedHandler : IRequestHandler<RequisitionApprovalGetAllPagedQuery, PagedResult<RequisitionApprovalResponse>>
    {
        private readonly IRequisitionApprovalService _requisitionApprovalService;

        public RequisitionApprovalGetAllPagedHandler(IRequisitionApprovalService requisitionApprovalService)
        {
            _requisitionApprovalService = requisitionApprovalService;
        }

        public async Task<PagedResult<RequisitionApprovalResponse>> Handle(RequisitionApprovalGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _requisitionApprovalService.GetPaginatedAsync(query.Request);
        }
    }
}
