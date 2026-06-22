using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Commands.DocumentTemplateCreate
{
    public class DocumentTemplateCreateHandler : IRequestHandler<DocumentTemplateCreateCommand, long>
    {
        private readonly IDocumentTemplateService _service;

        public DocumentTemplateCreateHandler(IDocumentTemplateService service)
        {
            _service = service;
        }

        public async Task<long> Handle(DocumentTemplateCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
