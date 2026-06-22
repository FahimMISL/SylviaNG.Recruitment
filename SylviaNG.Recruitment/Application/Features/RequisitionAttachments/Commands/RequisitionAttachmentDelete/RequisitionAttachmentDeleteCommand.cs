using MediatR;

namespace SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Commands.RequisitionAttachmentDelete
{
    public class RequisitionAttachmentDeleteCommand : IRequest<Unit>
    {
        public long RequisitionAttachmentId { get; set; }

        public RequisitionAttachmentDeleteCommand(long requisitionAttachmentId)
        {
            RequisitionAttachmentId = requisitionAttachmentId;
        }
    }
}
