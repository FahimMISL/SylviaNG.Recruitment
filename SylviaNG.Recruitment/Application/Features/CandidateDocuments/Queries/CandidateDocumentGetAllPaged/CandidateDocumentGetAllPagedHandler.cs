using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateDocuments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.CandidateDocuments.Queries.CandidateDocumentGetAllPaged
{
    public class CandidateDocumentGetAllPagedHandler : IRequestHandler<CandidateDocumentGetAllPagedQuery, PagedResult<CandidateDocumentResponse>>
    {
        private readonly ICandidateDocumentService _candidateDocumentService;

        public CandidateDocumentGetAllPagedHandler(ICandidateDocumentService candidateDocumentService)
        {
            _candidateDocumentService = candidateDocumentService;
        }

        public async Task<PagedResult<CandidateDocumentResponse>> Handle(CandidateDocumentGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _candidateDocumentService.GetPaginatedAsync(query.Request);
        }
    }
}
