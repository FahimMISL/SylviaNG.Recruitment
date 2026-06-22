using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Queries.RequisitionTemplateGetAll
{
    public class RequisitionTemplateGetAllHandler : IRequestHandler<RequisitionTemplateGetAllQuery, List<RequisitionTemplateResponse>>
    {
        private readonly IRequisitionTemplateService _service;

        public RequisitionTemplateGetAllHandler(IRequisitionTemplateService service)
        {
            _service = service;
        }

        public async Task<List<RequisitionTemplateResponse>> Handle(RequisitionTemplateGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
