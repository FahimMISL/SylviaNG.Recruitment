using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Models;

namespace SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Commands.RequisitionAttachmentCreate
{
    public class RequisitionAttachmentCreateCommand : IRequest<long>
    {
        public RequisitionAttachmentCreateRequest Request { get; set; }

        public RequisitionAttachmentCreateCommand(RequisitionAttachmentCreateRequest request)
        {
            Request = request;
        }
    }
}
