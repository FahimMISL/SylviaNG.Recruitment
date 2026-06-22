using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Commands.RequisitionAttachmentDelete
{
    public class RequisitionAttachmentDeleteHandler : IRequestHandler<RequisitionAttachmentDeleteCommand, Unit>
    {
        private readonly IRequisitionAttachmentService _service;

        public RequisitionAttachmentDeleteHandler(IRequisitionAttachmentService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(RequisitionAttachmentDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.RequisitionAttachmentId);
            return Unit.Value;
        }
    }
}
