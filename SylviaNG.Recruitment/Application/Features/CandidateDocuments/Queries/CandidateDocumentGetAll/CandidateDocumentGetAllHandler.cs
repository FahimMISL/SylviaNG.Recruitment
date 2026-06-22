using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateDocuments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateDocuments.Queries.CandidateDocumentGetAll
{
    public class CandidateDocumentGetAllHandler : IRequestHandler<CandidateDocumentGetAllQuery, List<CandidateDocumentResponse>>
    {
        private readonly ICandidateDocumentService _service;

        public CandidateDocumentGetAllHandler(ICandidateDocumentService service)
        {
            _service = service;
        }

        public async Task<List<CandidateDocumentResponse>> Handle(CandidateDocumentGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
