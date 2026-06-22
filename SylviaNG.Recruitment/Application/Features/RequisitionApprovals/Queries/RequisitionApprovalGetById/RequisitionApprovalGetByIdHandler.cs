using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Queries.RequisitionApprovalGetById
{
    public class RequisitionApprovalGetByIdHandler : IRequestHandler<RequisitionApprovalGetByIdQuery, RequisitionApprovalResponse>
    {
        private readonly IRequisitionApprovalService _service;

        public RequisitionApprovalGetByIdHandler(IRequisitionApprovalService service)
        {
            _service = service;
        }

        public async Task<RequisitionApprovalResponse> Handle(RequisitionApprovalGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.RequisitionApprovalId);
        }
    }
}
