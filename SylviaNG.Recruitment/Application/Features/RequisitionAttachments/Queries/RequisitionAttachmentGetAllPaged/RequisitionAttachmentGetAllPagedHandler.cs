using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Queries.RequisitionAttachmentGetAllPaged
{
    public class RequisitionAttachmentGetAllPagedHandler : IRequestHandler<RequisitionAttachmentGetAllPagedQuery, PagedResult<RequisitionAttachmentResponse>>
    {
        private readonly IRequisitionAttachmentService _requisitionAttachmentService;

        public RequisitionAttachmentGetAllPagedHandler(IRequisitionAttachmentService requisitionAttachmentService)
        {
            _requisitionAttachmentService = requisitionAttachmentService;
        }

        public async Task<PagedResult<RequisitionAttachmentResponse>> Handle(RequisitionAttachmentGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _requisitionAttachmentService.GetPaginatedAsync(query.Request);
        }
    }
}
