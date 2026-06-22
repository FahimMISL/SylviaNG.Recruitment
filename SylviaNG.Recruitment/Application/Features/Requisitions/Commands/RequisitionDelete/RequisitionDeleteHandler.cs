using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Requisitions.Commands.RequisitionDelete
{
    public class RequisitionDeleteHandler : IRequestHandler<RequisitionDeleteCommand, Unit>
    {
        private readonly IRequisitionService _service;

        public RequisitionDeleteHandler(IRequisitionService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(RequisitionDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.RequisitionId);
            return Unit.Value;
        }
    }
}
