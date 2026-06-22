using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Queries.DocumentTemplateVersionGetAllPaged
{
    public class DocumentTemplateVersionGetAllPagedHandler : IRequestHandler<DocumentTemplateVersionGetAllPagedQuery, PagedResult<DocumentTemplateVersionResponse>>
    {
        private readonly IDocumentTemplateVersionService _documentTemplateVersionService;

        public DocumentTemplateVersionGetAllPagedHandler(IDocumentTemplateVersionService documentTemplateVersionService)
        {
            _documentTemplateVersionService = documentTemplateVersionService;
        }

        public async Task<PagedResult<DocumentTemplateVersionResponse>> Handle(DocumentTemplateVersionGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _documentTemplateVersionService.GetPaginatedAsync(query.Request);
        }
    }
}
