using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Commands.DocumentTemplateDelete
{
    public class DocumentTemplateDeleteHandler : IRequestHandler<DocumentTemplateDeleteCommand, Unit>
    {
        private readonly IDocumentTemplateService _documentTemplateService;

        public DocumentTemplateDeleteHandler(IDocumentTemplateService documentTemplateService)
        {
            _documentTemplateService = documentTemplateService;
        }

        public async Task<Unit> Handle(DocumentTemplateDeleteCommand command, CancellationToken cancellationToken)
        {
            await _documentTemplateService.DeleteAsync(command.DocumentTemplateId);
            return Unit.Value;
        }
    }
}
