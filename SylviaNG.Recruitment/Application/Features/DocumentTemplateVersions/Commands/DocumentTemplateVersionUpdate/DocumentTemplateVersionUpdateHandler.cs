using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Commands.DocumentTemplateVersionUpdate
{
    public class DocumentTemplateVersionUpdateHandler : IRequestHandler<DocumentTemplateVersionUpdateCommand, Unit>
    {
        private readonly IDocumentTemplateVersionService _service;

        public DocumentTemplateVersionUpdateHandler(IDocumentTemplateVersionService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(DocumentTemplateVersionUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.DocumentTemplateVersionId, command.Request);
            return Unit.Value;
        }
    }
}
