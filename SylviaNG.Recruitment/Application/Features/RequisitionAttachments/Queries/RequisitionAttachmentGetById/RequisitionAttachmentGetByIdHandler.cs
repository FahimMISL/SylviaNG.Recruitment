using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Queries.RequisitionAttachmentGetById
{
    public class RequisitionAttachmentGetByIdHandler : IRequestHandler<RequisitionAttachmentGetByIdQuery, RequisitionAttachmentResponse>
    {
        private readonly IRequisitionAttachmentService _service;

        public RequisitionAttachmentGetByIdHandler(IRequisitionAttachmentService service)
        {
            _service = service;
        }

        public async Task<RequisitionAttachmentResponse> Handle(RequisitionAttachmentGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.RequisitionAttachmentId);
        }
    }
}
