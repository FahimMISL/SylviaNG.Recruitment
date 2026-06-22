using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Commands.DocumentTemplateVersionDelete
{
    public class DocumentTemplateVersionDeleteHandler : IRequestHandler<DocumentTemplateVersionDeleteCommand, Unit>
    {
        private readonly IDocumentTemplateVersionService _service;

        public DocumentTemplateVersionDeleteHandler(IDocumentTemplateVersionService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(DocumentTemplateVersionDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.DocumentTemplateVersionId);
            return Unit.Value;
        }
    }
}
