using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Models;

namespace SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Commands.RequisitionApprovalUpdate
{
    public class RequisitionApprovalUpdateCommand : IRequest<Unit>
    {
        public long RequisitionApprovalId { get; set; }
        public RequisitionApprovalUpdateRequest Request { get; set; }

        public RequisitionApprovalUpdateCommand(long requisitionApprovalId, RequisitionApprovalUpdateRequest request)
        {
            RequisitionApprovalId = requisitionApprovalId;
            Request = request;
        }
    }
}
