using MediatR;

namespace SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Commands.RequisitionTemplateDelete
{
    public class RequisitionTemplateDeleteCommand : IRequest<Unit>
    {
        public long RequisitionTemplateId { get; set; }

        public RequisitionTemplateDeleteCommand(long requisitionTemplateId)
        {
            RequisitionTemplateId = requisitionTemplateId;
        }
    }
}
