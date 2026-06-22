using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Queries.RequisitionAttachmentGetAll
{
    public class RequisitionAttachmentGetAllHandler : IRequestHandler<RequisitionAttachmentGetAllQuery, List<RequisitionAttachmentResponse>>
    {
        private readonly IRequisitionAttachmentService _service;

        public RequisitionAttachmentGetAllHandler(IRequisitionAttachmentService service)
        {
            _service = service;
        }

        public async Task<List<RequisitionAttachmentResponse>> Handle(RequisitionAttachmentGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
