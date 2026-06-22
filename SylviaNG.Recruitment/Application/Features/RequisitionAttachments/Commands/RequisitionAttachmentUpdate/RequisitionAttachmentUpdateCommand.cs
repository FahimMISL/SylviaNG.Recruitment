using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Models;

namespace SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Commands.RequisitionAttachmentUpdate
{
    public class RequisitionAttachmentUpdateCommand : IRequest<Unit>
    {
        public long RequisitionAttachmentId { get; set; }
        public RequisitionAttachmentUpdateRequest Request { get; set; }

        public RequisitionAttachmentUpdateCommand(long requisitionAttachmentId, RequisitionAttachmentUpdateRequest request)
        {
            RequisitionAttachmentId = requisitionAttachmentId;
            Request = request;
        }
    }
}
