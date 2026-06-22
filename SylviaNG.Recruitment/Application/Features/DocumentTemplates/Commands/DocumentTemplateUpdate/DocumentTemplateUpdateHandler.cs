using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Commands.DocumentTemplateUpdate
{
    public class DocumentTemplateUpdateHandler : IRequestHandler<DocumentTemplateUpdateCommand, Unit>
    {
        private readonly IDocumentTemplateService _service;

        public DocumentTemplateUpdateHandler(IDocumentTemplateService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(DocumentTemplateUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.DocumentTemplateId, command.Request);
            return Unit.Value;
        }
    }
}
