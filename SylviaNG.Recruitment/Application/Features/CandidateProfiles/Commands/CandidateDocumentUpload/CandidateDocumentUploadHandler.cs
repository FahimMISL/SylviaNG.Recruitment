using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateDocumentUpload
{
    public class CandidateDocumentUploadHandler : IRequestHandler<CandidateDocumentUploadCommand, CandidateDocumentResponse>
    {
        private readonly ICandidateDocumentService _candidateDocumentService;

        public CandidateDocumentUploadHandler(ICandidateDocumentService candidateDocumentService)
        {
            _candidateDocumentService = candidateDocumentService;
        }

        public async Task<CandidateDocumentResponse> Handle(CandidateDocumentUploadCommand command, CancellationToken cancellationToken)
        {
            return await _candidateDocumentService.UploadAsync(command.Request);
        }
    }
}
