using MediatR;

namespace SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Commands.RequisitionApprovalDelete
{
    public class RequisitionApprovalDeleteCommand : IRequest<Unit>
    {
        public long RequisitionApprovalId { get; set; }

        public RequisitionApprovalDeleteCommand(long requisitionApprovalId)
        {
            RequisitionApprovalId = requisitionApprovalId;
        }
    }
}
