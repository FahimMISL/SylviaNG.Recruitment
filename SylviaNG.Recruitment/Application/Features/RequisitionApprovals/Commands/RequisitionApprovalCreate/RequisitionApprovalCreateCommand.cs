using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Models;

namespace SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Commands.RequisitionApprovalCreate
{
    public class RequisitionApprovalCreateCommand : IRequest<long>
    {
        public RequisitionApprovalCreateRequest Request { get; set; }

        public RequisitionApprovalCreateCommand(RequisitionApprovalCreateRequest request)
        {
            Request = request;
        }
    }
}
