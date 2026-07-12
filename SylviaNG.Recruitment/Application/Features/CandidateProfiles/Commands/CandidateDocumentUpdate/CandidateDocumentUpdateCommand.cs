using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateDocumentUpdate
{
    public class CandidateDocumentUpdateCommand : IRequest<CandidateDocumentResponse>
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
