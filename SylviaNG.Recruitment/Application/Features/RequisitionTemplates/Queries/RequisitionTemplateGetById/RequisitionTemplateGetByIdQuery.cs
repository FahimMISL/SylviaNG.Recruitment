using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Models;

namespace SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Queries.RequisitionTemplateGetById
{
    public class RequisitionTemplateGetByIdQuery : IRequest<RequisitionTemplateResponse>
    {
        public long RequisitionTemplateId { get; set; }

        public RequisitionTemplateGetByIdQuery(long requisitionTemplateId)
        {
            RequisitionTemplateId = requisitionTemplateId;
        }
    }
}
