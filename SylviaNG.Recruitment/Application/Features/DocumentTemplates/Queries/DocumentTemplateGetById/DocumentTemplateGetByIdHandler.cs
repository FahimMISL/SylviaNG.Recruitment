using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Queries.DocumentTemplateGetById
{
    public class DocumentTemplateGetByIdHandler : IRequestHandler<DocumentTemplateGetByIdQuery, DocumentTemplateResponse>
    {
        private readonly IDocumentTemplateService _service;

        public DocumentTemplateGetByIdHandler(IDocumentTemplateService service)
        {
            _service = service;
        }

        public async Task<DocumentTemplateResponse> Handle(DocumentTemplateGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.DocumentTemplateId);
        }
    }
}
