using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Commands.DocumentTemplateDelete
{
    public class DocumentTemplateDeleteHandler : IRequestHandler<DocumentTemplateDeleteCommand, Unit>
    {
        private readonly IDocumentTemplateService _service;

        public DocumentTemplateDeleteHandler(IDocumentTemplateService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(DocumentTemplateDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.DocumentTemplateId);
            return Unit.Value;
        }
    }
}
