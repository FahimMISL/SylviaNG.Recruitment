using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateDocumentGetAll
{
    public class CandidateDocumentGetAllHandler : IRequestHandler<CandidateDocumentGetAllQuery, List<CandidateDocumentResponse>>
    {
        private readonly ICandidateDocumentService _candidateDocumentService;

        public CandidateDocumentGetAllHandler(ICandidateDocumentService candidateDocumentService)
        {
            _candidateDocumentService = candidateDocumentService;
        }

        public async Task<List<CandidateDocumentResponse>> Handle(CandidateDocumentGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _candidateDocumentService.GetAllForCurrentCandidateAsync();
        }
    }
}
