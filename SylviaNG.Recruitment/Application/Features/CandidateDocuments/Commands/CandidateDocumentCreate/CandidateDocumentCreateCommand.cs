using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateDocuments.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateDocuments.Commands.CandidateDocumentCreate
{
    public class CandidateDocumentCreateCommand : IRequest<long>
    {
        public CandidateDocumentCreateRequest Request { get; set; }

        public CandidateDocumentCreateCommand(CandidateDocumentCreateRequest request)
        {
            Request = request;
        }
    }
}
