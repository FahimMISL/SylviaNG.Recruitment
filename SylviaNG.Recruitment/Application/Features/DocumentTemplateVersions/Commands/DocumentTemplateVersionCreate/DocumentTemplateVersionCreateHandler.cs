using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Commands.DocumentTemplateVersionCreate
{
    public class DocumentTemplateVersionCreateHandler : IRequestHandler<DocumentTemplateVersionCreateCommand, long>
    {
        private readonly IDocumentTemplateVersionService _service;

        public DocumentTemplateVersionCreateHandler(IDocumentTemplateVersionService service)
        {
            _service = service;
        }

        public async Task<long> Handle(DocumentTemplateVersionCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
