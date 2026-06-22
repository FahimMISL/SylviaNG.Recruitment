using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Commands.RequisitionApprovalCreate
{
    public class RequisitionApprovalCreateHandler : IRequestHandler<RequisitionApprovalCreateCommand, long>
    {
        private readonly IRequisitionApprovalService _service;

        public RequisitionApprovalCreateHandler(IRequisitionApprovalService service)
        {
            _service = service;
        }

        public async Task<long> Handle(RequisitionApprovalCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
