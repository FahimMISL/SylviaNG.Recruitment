using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Commands.RequisitionTemplateCreate
{
    public class RequisitionTemplateCreateHandler : IRequestHandler<RequisitionTemplateCreateCommand, long>
    {
        private readonly IRequisitionTemplateService _service;

        public RequisitionTemplateCreateHandler(IRequisitionTemplateService service)
        {
            _service = service;
        }

        public async Task<long> Handle(RequisitionTemplateCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
