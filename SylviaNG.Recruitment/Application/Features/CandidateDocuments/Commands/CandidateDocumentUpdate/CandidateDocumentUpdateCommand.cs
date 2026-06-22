using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateDocuments.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateDocuments.Commands.CandidateDocumentUpdate
{
    public class CandidateDocumentUpdateCommand : IRequest<Unit>
    {
        public long CandidateDocumentId { get; set; }
        public CandidateDocumentUpdateRequest Request { get; set; }

        public CandidateDocumentUpdateCommand(long candidateDocumentId, CandidateDocumentUpdateRequest request)
        {
            CandidateDocumentId = candidateDocumentId;
            Request = request;
        }
    }
}
