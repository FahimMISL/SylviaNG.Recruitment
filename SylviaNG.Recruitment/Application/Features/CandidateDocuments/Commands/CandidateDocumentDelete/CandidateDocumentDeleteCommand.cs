using MediatR;

namespace SylviaNG.Recruitment.Application.Features.CandidateDocuments.Commands.CandidateDocumentDelete
{
    public class CandidateDocumentDeleteCommand : IRequest<Unit>
    {
        public long CandidateDocumentId { get; set; }

        public CandidateDocumentDeleteCommand(long candidateDocumentId)
        {
            CandidateDocumentId = candidateDocumentId;
        }
    }
}
