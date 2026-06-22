using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Commands.RequisitionTemplateDelete
{
    public class RequisitionTemplateDeleteHandler : IRequestHandler<RequisitionTemplateDeleteCommand, Unit>
    {
        private readonly IRequisitionTemplateService _service;

        public RequisitionTemplateDeleteHandler(IRequisitionTemplateService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(RequisitionTemplateDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.RequisitionTemplateId);
            return Unit.Value;
        }
    }
}
