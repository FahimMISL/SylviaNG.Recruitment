using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Commands.RequisitionTemplateUpdate
{
    public class RequisitionTemplateUpdateHandler : IRequestHandler<RequisitionTemplateUpdateCommand, Unit>
    {
        private readonly IRequisitionTemplateService _service;

        public RequisitionTemplateUpdateHandler(IRequisitionTemplateService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(RequisitionTemplateUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.RequisitionTemplateId, command.Request);
            return Unit.Value;
        }
    }
}
