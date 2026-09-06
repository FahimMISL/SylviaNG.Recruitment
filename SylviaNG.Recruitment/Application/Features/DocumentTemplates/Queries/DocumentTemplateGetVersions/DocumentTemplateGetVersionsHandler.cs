using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Queries.DocumentTemplateGetVersions
{
    public class DocumentTemplateGetVersionsHandler : IRequestHandler<DocumentTemplateGetVersionsQuery, List<DocumentTemplateVersionResponse>>
    {
        private readonly IDocumentTemplateService _documentTemplateService;

        public DocumentTemplateGetVersionsHandler(IDocumentTemplateService documentTemplateService)
        {
            _documentTemplateService = documentTemplateService;
        }

        public async Task<List<DocumentTemplateVersionResponse>> Handle(DocumentTemplateGetVersionsQuery query, CancellationToken cancellationToken)
        {
            return await _documentTemplateService.GetVersionsAsync(query.DocumentTemplateId);
        }
    }
}
