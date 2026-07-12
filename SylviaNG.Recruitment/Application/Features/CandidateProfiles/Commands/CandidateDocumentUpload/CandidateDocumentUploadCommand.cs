using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateDocumentUpload
{
    public class CandidateDocumentUploadCommand : IRequest<CandidateDocumentResponse>
    {
        public CandidateDocumentUploadRequest Request { get; set; }

        public CandidateDocumentUploadCommand(CandidateDocumentUploadRequest request)
        {
            Request = request;
        }
    }
}
