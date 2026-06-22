using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Models;

namespace SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Queries.RequisitionAttachmentGetAll
{
    public class RequisitionAttachmentGetAllQuery : IRequest<List<RequisitionAttachmentResponse>>
    {
    }
}
