using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Queries.DocumentAcceptanceGetAllPaged
{
    public class DocumentAcceptanceGetAllPagedHandler : IRequestHandler<DocumentAcceptanceGetAllPagedQuery, PagedResult<DocumentAcceptanceResponse>>
    {
        private readonly IDocumentAcceptanceService _documentAcceptanceService;

        public DocumentAcceptanceGetAllPagedHandler(IDocumentAcceptanceService documentAcceptanceService)
        {
            _documentAcceptanceService = documentAcceptanceService;
        }

        public async Task<PagedResult<DocumentAcceptanceResponse>> Handle(DocumentAcceptanceGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _documentAcceptanceService.GetPaginatedAsync(query.Request);
        }
    }
}
