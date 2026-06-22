using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Commands.RequisitionApprovalDelete
{
    public class RequisitionApprovalDeleteHandler : IRequestHandler<RequisitionApprovalDeleteCommand, Unit>
    {
        private readonly IRequisitionApprovalService _service;

        public RequisitionApprovalDeleteHandler(IRequisitionApprovalService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(RequisitionApprovalDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.RequisitionApprovalId);
            return Unit.Value;
        }
    }
}
