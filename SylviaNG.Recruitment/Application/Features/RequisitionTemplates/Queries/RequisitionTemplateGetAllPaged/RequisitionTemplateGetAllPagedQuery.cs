using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Queries.RequisitionTemplateGetAllPaged
{
    public class RequisitionTemplateGetAllPagedQuery : IRequest<PagedResult<RequisitionTemplateResponse>>
    {
        public PagedRequest Request { get; set; }

        public RequisitionTemplateGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
