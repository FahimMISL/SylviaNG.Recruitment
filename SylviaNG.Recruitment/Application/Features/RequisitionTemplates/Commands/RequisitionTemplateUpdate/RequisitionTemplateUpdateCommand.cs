using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Models;

namespace SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Commands.RequisitionTemplateUpdate
{
    public class RequisitionTemplateUpdateCommand : IRequest<Unit>
    {
        public long RequisitionTemplateId { get; set; }
        public RequisitionTemplateUpdateRequest Request { get; set; }

        public RequisitionTemplateUpdateCommand(long requisitionTemplateId, RequisitionTemplateUpdateRequest request)
        {
            RequisitionTemplateId = requisitionTemplateId;
            Request = request;
        }
    }
}
