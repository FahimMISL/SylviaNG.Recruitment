using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Queries.RequisitionAttachmentGetAllPaged
{
    public class RequisitionAttachmentGetAllPagedQuery : IRequest<PagedResult<RequisitionAttachmentResponse>>
    {
        public PagedRequest Request { get; set; }

        public RequisitionAttachmentGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
