using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Models;

namespace SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Queries.RequisitionApprovalGetById
{
    public class RequisitionApprovalGetByIdQuery : IRequest<RequisitionApprovalResponse>
    {
        public long RequisitionApprovalId { get; set; }

        public RequisitionApprovalGetByIdQuery(long requisitionApprovalId)
        {
            RequisitionApprovalId = requisitionApprovalId;
        }
    }
}
