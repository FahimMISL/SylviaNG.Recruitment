using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateDocuments.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateDocuments.Queries.CandidateDocumentGetById
{
    public class CandidateDocumentGetByIdQuery : IRequest<CandidateDocumentResponse>
    {
        public long CandidateDocumentId { get; set; }

        public CandidateDocumentGetByIdQuery(long candidateDocumentId)
        {
            CandidateDocumentId = candidateDocumentId;
        }
    }
}
