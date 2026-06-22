using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Queries.DocumentTemplateVersionGetAll
{
    public class DocumentTemplateVersionGetAllHandler : IRequestHandler<DocumentTemplateVersionGetAllQuery, List<DocumentTemplateVersionResponse>>
    {
        private readonly IDocumentTemplateVersionService _service;

        public DocumentTemplateVersionGetAllHandler(IDocumentTemplateVersionService service)
        {
            _service = service;
        }

        public async Task<List<DocumentTemplateVersionResponse>> Handle(DocumentTemplateVersionGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
