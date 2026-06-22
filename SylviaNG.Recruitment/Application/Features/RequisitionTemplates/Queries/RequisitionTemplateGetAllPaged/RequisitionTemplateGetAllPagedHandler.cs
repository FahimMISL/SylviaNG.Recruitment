using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Queries.RequisitionTemplateGetAllPaged
{
    public class RequisitionTemplateGetAllPagedHandler : IRequestHandler<RequisitionTemplateGetAllPagedQuery, PagedResult<RequisitionTemplateResponse>>
    {
        private readonly IRequisitionTemplateService _requisitionTemplateService;

        public RequisitionTemplateGetAllPagedHandler(IRequisitionTemplateService requisitionTemplateService)
        {
            _requisitionTemplateService = requisitionTemplateService;
        }

        public async Task<PagedResult<RequisitionTemplateResponse>> Handle(RequisitionTemplateGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _requisitionTemplateService.GetPaginatedAsync(query.Request);
        }
    }
}
