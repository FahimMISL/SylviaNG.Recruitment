using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Models;

namespace SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Queries.RequisitionApprovalGetAll
{
    public class RequisitionApprovalGetAllQuery : IRequest<List<RequisitionApprovalResponse>>
    {
    }
}
