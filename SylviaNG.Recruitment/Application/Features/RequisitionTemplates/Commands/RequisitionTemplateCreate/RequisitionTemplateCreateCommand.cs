using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Models;

namespace SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Commands.RequisitionTemplateCreate
{
    public class RequisitionTemplateCreateCommand : IRequest<long>
    {
        public RequisitionTemplateCreateRequest Request { get; set; }

        public RequisitionTemplateCreateCommand(RequisitionTemplateCreateRequest request)
        {
            Request = request;
        }
    }
}
