using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Queries.RequisitionApprovalGetAll
{
    public class RequisitionApprovalGetAllHandler : IRequestHandler<RequisitionApprovalGetAllQuery, List<RequisitionApprovalResponse>>
    {
        private readonly IRequisitionApprovalService _service;

        public RequisitionApprovalGetAllHandler(IRequisitionApprovalService service)
        {
            _service = service;
        }

        public async Task<List<RequisitionApprovalResponse>> Handle(RequisitionApprovalGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
