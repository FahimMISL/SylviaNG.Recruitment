using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Queries.DocumentTemplateVersionGetById
{
    public class DocumentTemplateVersionGetByIdHandler : IRequestHandler<DocumentTemplateVersionGetByIdQuery, DocumentTemplateVersionResponse>
    {
        private readonly IDocumentTemplateVersionService _service;

        public DocumentTemplateVersionGetByIdHandler(IDocumentTemplateVersionService service)
        {
            _service = service;
        }

        public async Task<DocumentTemplateVersionResponse> Handle(DocumentTemplateVersionGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.DocumentTemplateVersionId);
        }
    }
}
