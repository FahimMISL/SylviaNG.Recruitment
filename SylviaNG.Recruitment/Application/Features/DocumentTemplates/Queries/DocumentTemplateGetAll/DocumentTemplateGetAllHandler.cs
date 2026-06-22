using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Queries.DocumentTemplateGetAll
{
    public class DocumentTemplateGetAllHandler : IRequestHandler<DocumentTemplateGetAllQuery, List<DocumentTemplateResponse>>
    {
        private readonly IDocumentTemplateService _service;

        public DocumentTemplateGetAllHandler(IDocumentTemplateService service)
        {
            _service = service;
        }

        public async Task<List<DocumentTemplateResponse>> Handle(DocumentTemplateGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
