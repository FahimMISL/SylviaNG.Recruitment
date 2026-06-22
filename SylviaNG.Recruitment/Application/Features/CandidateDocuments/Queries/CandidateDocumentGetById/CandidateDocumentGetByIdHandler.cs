using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateDocuments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateDocuments.Queries.CandidateDocumentGetById
{
    public class CandidateDocumentGetByIdHandler : IRequestHandler<CandidateDocumentGetByIdQuery, CandidateDocumentResponse>
    {
        private readonly ICandidateDocumentService _service;

        public CandidateDocumentGetByIdHandler(ICandidateDocumentService service)
        {
            _service = service;
        }

        public async Task<CandidateDocumentResponse> Handle(CandidateDocumentGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.CandidateDocumentId);
        }
    }
}
