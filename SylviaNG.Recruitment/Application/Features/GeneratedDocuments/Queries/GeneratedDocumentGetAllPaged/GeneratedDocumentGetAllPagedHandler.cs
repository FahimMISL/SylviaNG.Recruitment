using MediatR;
using SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Queries.GeneratedDocumentGetAllPaged
{
    public class GeneratedDocumentGetAllPagedHandler : IRequestHandler<GeneratedDocumentGetAllPagedQuery, PagedResult<GeneratedDocumentResponse>>
    {
        private readonly IGeneratedDocumentService _generatedDocumentService;

        public GeneratedDocumentGetAllPagedHandler(IGeneratedDocumentService generatedDocumentService)
        {
            _generatedDocumentService = generatedDocumentService;
        }

        public async Task<PagedResult<GeneratedDocumentResponse>> Handle(GeneratedDocumentGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _generatedDocumentService.GetPaginatedAsync(query.Request);
        }
    }
}
