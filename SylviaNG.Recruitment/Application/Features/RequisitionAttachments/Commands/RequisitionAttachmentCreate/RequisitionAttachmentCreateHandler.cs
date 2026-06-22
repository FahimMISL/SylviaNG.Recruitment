using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Commands.RequisitionAttachmentCreate
{
    public class RequisitionAttachmentCreateHandler : IRequestHandler<RequisitionAttachmentCreateCommand, long>
    {
        private readonly IRequisitionAttachmentService _service;

        public RequisitionAttachmentCreateHandler(IRequisitionAttachmentService service)
        {
            _service = service;
        }

        public async Task<long> Handle(RequisitionAttachmentCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
