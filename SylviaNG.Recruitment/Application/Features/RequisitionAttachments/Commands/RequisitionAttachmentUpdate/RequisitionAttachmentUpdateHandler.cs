using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Commands.RequisitionAttachmentUpdate
{
    public class RequisitionAttachmentUpdateHandler : IRequestHandler<RequisitionAttachmentUpdateCommand, Unit>
    {
        private readonly IRequisitionAttachmentService _service;

        public RequisitionAttachmentUpdateHandler(IRequisitionAttachmentService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(RequisitionAttachmentUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.RequisitionAttachmentId, command.Request);
            return Unit.Value;
        }
    }
}
