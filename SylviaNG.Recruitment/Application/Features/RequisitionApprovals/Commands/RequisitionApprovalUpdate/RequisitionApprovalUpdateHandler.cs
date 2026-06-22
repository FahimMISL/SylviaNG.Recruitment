using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Commands.RequisitionApprovalUpdate
{
    public class RequisitionApprovalUpdateHandler : IRequestHandler<RequisitionApprovalUpdateCommand, Unit>
    {
        private readonly IRequisitionApprovalService _service;

        public RequisitionApprovalUpdateHandler(IRequisitionApprovalService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(RequisitionApprovalUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.RequisitionApprovalId, command.Request);
            return Unit.Value;
        }
    }
}
