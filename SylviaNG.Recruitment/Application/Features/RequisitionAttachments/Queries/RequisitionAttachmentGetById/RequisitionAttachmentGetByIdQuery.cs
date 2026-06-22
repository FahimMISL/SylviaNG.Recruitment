using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Models;

namespace SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Queries.RequisitionAttachmentGetById
{
    public class RequisitionAttachmentGetByIdQuery : IRequest<RequisitionAttachmentResponse>
    {
        public long RequisitionAttachmentId { get; set; }

        public RequisitionAttachmentGetByIdQuery(long requisitionAttachmentId)
        {
            RequisitionAttachmentId = requisitionAttachmentId;
        }
    }
}
