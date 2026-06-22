using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Models;

namespace SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Queries.RequisitionTemplateGetAll
{
    public class RequisitionTemplateGetAllQuery : IRequest<List<RequisitionTemplateResponse>>
    {
    }
}
