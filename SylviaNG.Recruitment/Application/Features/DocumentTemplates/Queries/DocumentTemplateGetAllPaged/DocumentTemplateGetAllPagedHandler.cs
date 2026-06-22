using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Queries.DocumentTemplateGetAllPaged
{
    public class DocumentTemplateGetAllPagedHandler : IRequestHandler<DocumentTemplateGetAllPagedQuery, PagedResult<DocumentTemplateResponse>>
    {
        private readonly IDocumentTemplateService _documentTemplateService;

        public DocumentTemplateGetAllPagedHandler(IDocumentTemplateService documentTemplateService)
        {
            _documentTemplateService = documentTemplateService;
        }

        public async Task<PagedResult<DocumentTemplateResponse>> Handle(DocumentTemplateGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _documentTemplateService.GetPaginatedAsync(query.Request);
        }
    }
}
