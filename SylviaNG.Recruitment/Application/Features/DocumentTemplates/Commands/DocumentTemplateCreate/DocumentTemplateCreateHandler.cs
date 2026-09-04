using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Commands.DocumentTemplateCreate
{
    public class DocumentTemplateCreateHandler : IRequestHandler<DocumentTemplateCreateCommand, long>
    {
        private readonly IDocumentTemplateService _documentTemplateService;

        public DocumentTemplateCreateHandler(IDocumentTemplateService documentTemplateService)
        {
            _documentTemplateService = documentTemplateService;
        }

        public async Task<long> Handle(DocumentTemplateCreateCommand command, CancellationToken cancellationToken)
        {
            return await _documentTemplateService.CreateAsync(command.Request);
        }
    }
}
