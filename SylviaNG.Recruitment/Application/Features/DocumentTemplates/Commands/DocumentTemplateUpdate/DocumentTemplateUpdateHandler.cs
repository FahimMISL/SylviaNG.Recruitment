using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Commands.DocumentTemplateUpdate
{
    public class DocumentTemplateUpdateHandler : IRequestHandler<DocumentTemplateUpdateCommand, Unit>
    {
        private readonly IDocumentTemplateService _documentTemplateService;

        public DocumentTemplateUpdateHandler(IDocumentTemplateService documentTemplateService)
        {
            _documentTemplateService = documentTemplateService;
        }

        public async Task<Unit> Handle(DocumentTemplateUpdateCommand command, CancellationToken cancellationToken)
        {
            await _documentTemplateService.UpdateAsync(command.DocumentTemplateId, command.Request);
            return Unit.Value;
        }
    }
}
