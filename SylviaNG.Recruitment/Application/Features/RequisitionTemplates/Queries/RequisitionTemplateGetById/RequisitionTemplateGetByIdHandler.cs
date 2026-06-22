using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Queries.RequisitionTemplateGetById
{
    public class RequisitionTemplateGetByIdHandler : IRequestHandler<RequisitionTemplateGetByIdQuery, RequisitionTemplateResponse>
    {
        private readonly IRequisitionTemplateService _service;

        public RequisitionTemplateGetByIdHandler(IRequisitionTemplateService service)
        {
            _service = service;
        }

        public async Task<RequisitionTemplateResponse> Handle(RequisitionTemplateGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.RequisitionTemplateId);
        }
    }
}
